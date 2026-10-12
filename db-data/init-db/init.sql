CREATE TABLE users (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    username VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    postal_code VARCHAR(20) NOT NULL,
    balance DECIMAL(18, 2) DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE UNIQUE INDEX users_username_lower_idx ON users (lower(username));


CREATE TABLE business_customers (
    user_id INT PRIMARY KEY REFERENCES users(id),
    cvr VARCHAR(20) NOT NULL,
    credit DECIMAL(18, 2) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE private_customers (
    user_id INT PRIMARY KEY REFERENCES users(id),
    cpr VARCHAR(20) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TYPE LicenceType AS ENUM ('A','B','C','D','BE','CE','DE');

CREATE TYPE FuelType AS ENUM ('Diesel','Petrol','Electric','Hydrogen');

CREATE TABLE vehicles (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    kilometers DOUBLE PRECISION NOT NULL,
    release_year INT NOT NULL,
    registration_number VARCHAR(10) UNIQUE NOT NULL,
    base_price DECIMAL(18, 2) NOT NULL,
    tow_bar BOOLEAN NOT NULL,
    engine_size DOUBLE PRECISION NOT NULL,
    km_per_liter DOUBLE PRECISION NOT NULL,
    fuel_type FuelType NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP

);

CREATE TABLE heavy_vehicles (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    vehicle_id INT REFERENCES vehicles(id) ON DELETE CASCADE,
    weight DOUBLE PRECISION NOT NULL,
    height DOUBLE PRECISION NOT NULL,
    length DOUBLE PRECISION NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP

);

CREATE TABLE semi_trucks (
    heavy_vehicle_id INT PRIMARY KEY REFERENCES heavy_vehicles(id) ON DELETE CASCADE,
    cargo_capacity DOUBLE PRECISION NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE buses (
    heavy_vehicle_id INT PRIMARY KEY REFERENCES heavy_vehicles(id) ON DELETE CASCADE,
    seat_count INT NOT NULL,
    bed_count INT NOT NULL,
    toilet BOOLEAN NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE personal_cars (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    seat_count INT NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,

    vehicle_id INT NOT NULL REFERENCES vehicles(id) ON DELETE CASCADE
);

CREATE TABLE business_personal_cars (
    car_id INT PRIMARY KEY REFERENCES personal_cars(id) ON DELETE CASCADE,
    cargo_capacity DOUBLE PRECISION NOT NULL,
    roll_cage BOOLEAN NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE private_personal_cars (
    car_id INT PRIMARY KEY REFERENCES personal_cars(id) ON DELETE CASCADE,
    isofix BOOLEAN NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE auctions (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    seller_id INT NOT NULL REFERENCES users(id),
    vehicle_id INT NOT NULL REFERENCES vehicles(id),
    minimum_price DECIMAL(18, 2) NOT NULL,
    is_sold BOOLEAN NOT NULL DEFAULT FALSE,
    end_time TIMESTAMPTZ NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE bids (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    auction_id INT NOT NULL REFERENCES auctions(id) ON DELETE CASCADE,
    bidder_id INT NOT NULL REFERENCES users(id),
    amount DECIMAL(18, 2) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);


-- D3: user management is exposed to the application as database functions,
-- so the application never composes its own DML against the users table.

CREATE FUNCTION register_user(p_username VARCHAR, p_password_hash VARCHAR, p_postal_code VARCHAR)
RETURNS SETOF users
LANGUAGE sql
AS $$
    INSERT INTO users (username, password_hash, postal_code)
    VALUES (p_username, p_password_hash, p_postal_code)
    RETURNING *;
$$;

CREATE FUNCTION get_user_by_username(p_username VARCHAR)
RETURNS SETOF users
LANGUAGE sql
STABLE
AS $$
    SELECT * FROM users WHERE lower(username) = lower(p_username);
$$;


CREATE OR REPLACE FUNCTION add_private_personal_car(
    p_name varchar,
    p_kilometers double precision,
    p_release_year integer,
    p_registration_number varchar,
    p_base_price numeric,
    p_tow_bar boolean,
    p_engine_size double precision,
    p_km_per_liter double precision,
    p_fuel_type fueltype,
    p_seat_count integer,
    p_isofix boolean
)
RETURNS integer
LANGUAGE plpgsql
AS $$
DECLARE
    v_vehicle_id integer;
    v_car_id integer;
BEGIN
    INSERT INTO vehicles (
        name, kilometers, release_year,
        registration_number, base_price, tow_bar,
        engine_size, km_per_liter, fuel_type
    )
    VALUES (
        p_name, p_kilometers, p_release_year,
        p_registration_number, p_base_price, p_tow_bar,
        p_engine_size, p_km_per_liter, p_fuel_type
    )
    RETURNING id INTO v_vehicle_id;

    INSERT INTO personal_cars (vehicle_id, seat_count)
    VALUES (v_vehicle_id, p_seat_count)
    RETURNING id INTO v_car_id;

    INSERT INTO private_personal_cars (car_id, isofix)
    VALUES (v_car_id, p_isofix);

    RETURN v_vehicle_id;
END;
$$;

CREATE OR REPLACE FUNCTION add_business_personal_car(
    p_name varchar,
    p_kilometers double precision,
    p_release_year integer,
    p_registration_number varchar,
    p_base_price numeric,
    p_tow_bar boolean,
    p_engine_size double precision,
    p_km_per_liter double precision,
    p_fuel_type fueltype,
    p_seat_count integer,
    p_cargo_capacity double precision,
    p_roll_cage boolean
)
RETURNS integer
LANGUAGE plpgsql
AS $$
DECLARE
    v_vehicle_id integer;
    v_car_id integer;
BEGIN
    INSERT INTO vehicles (
        name, kilometers, release_year,
        registration_number, base_price, tow_bar,
        engine_size, km_per_liter, fuel_type
    )
    VALUES (
        p_name, p_kilometers, p_release_year,
        p_registration_number, p_base_price, p_tow_bar,
        p_engine_size, p_km_per_liter, p_fuel_type
    )
    RETURNING id INTO v_vehicle_id;

    INSERT INTO personal_cars (vehicle_id, seat_count)
    VALUES (v_vehicle_id, p_seat_count)
    RETURNING id INTO v_car_id;

    INSERT INTO business_personal_cars (
        car_id, cargo_capacity, roll_cage
    )
    VALUES (
        v_car_id, p_cargo_capacity, p_roll_cage
    );

    RETURN v_vehicle_id;
END;
$$;

CREATE OR REPLACE FUNCTION add_semi_truck(
    p_name varchar,
    p_kilometers double precision,
    p_release_year integer,
    p_registration_number varchar,
    p_base_price numeric,
    p_tow_bar boolean,
    p_engine_size double precision,
    p_km_per_liter double precision,
    p_fuel_type fueltype,
    p_weight double precision,
    p_height double precision,
    p_length double precision,
    p_cargo_capacity double precision
)
RETURNS integer
LANGUAGE plpgsql
AS $$
DECLARE
    v_vehicle_id integer;
    v_heavy_vehicle_id integer;
BEGIN
    INSERT INTO vehicles (
        name, kilometers, release_year,
        registration_number, base_price, tow_bar,
        engine_size, km_per_liter, fuel_type
    )
    VALUES (
        p_name, p_kilometers, p_release_year,
        p_registration_number, p_base_price, p_tow_bar,
        p_engine_size, p_km_per_liter, p_fuel_type
    )
    RETURNING id INTO v_vehicle_id;

    INSERT INTO heavy_vehicles (
        vehicle_id, weight, height, length
    )
    VALUES (
        v_vehicle_id, p_weight, p_height, p_length
    )
    RETURNING id INTO v_heavy_vehicle_id;

    INSERT INTO semi_trucks (
        heavy_vehicle_id, cargo_capacity
    )
    VALUES (
        v_heavy_vehicle_id, p_cargo_capacity
    );

    RETURN v_vehicle_id;
END;
$$;

CREATE OR REPLACE FUNCTION add_bus(
    p_name varchar,
    p_kilometers double precision,
    p_release_year integer,
    p_registration_number varchar,
    p_base_price numeric,
    p_tow_bar boolean,
    p_engine_size double precision,
    p_km_per_liter double precision,
    p_fuel_type fueltype,
    p_weight double precision,
    p_height double precision,
    p_length double precision,
    p_seat_count integer,
    p_bed_count integer,
    p_toilet boolean
)
RETURNS integer
LANGUAGE plpgsql
AS $$
DECLARE
    v_vehicle_id integer;
    v_heavy_vehicle_id integer;
BEGIN
    INSERT INTO vehicles (
        name, kilometers, release_year,
        registration_number, base_price, tow_bar,
        engine_size, km_per_liter, fuel_type
    )
    VALUES (
        p_name, p_kilometers, p_release_year,
        p_registration_number, p_base_price, p_tow_bar,
        p_engine_size, p_km_per_liter, p_fuel_type
    )
    RETURNING id INTO v_vehicle_id;

    INSERT INTO heavy_vehicles (
        vehicle_id, weight, height, length
    )
    VALUES (
        v_vehicle_id, p_weight, p_height, p_length
    )
    RETURNING id INTO v_heavy_vehicle_id;

    INSERT INTO buses (
        heavy_vehicle_id, seat_count, bed_count, toilet
    )
    VALUES (
        v_heavy_vehicle_id, p_seat_count, p_bed_count, p_toilet
    );

    RETURN v_vehicle_id;
END;
$$;

CREATE OR REPLACE FUNCTION get_vehicle_by_id(p_id integer)
RETURNS TABLE (
    id integer,
    name varchar,
    release_year integer,
    registration_number varchar,
    base_price numeric,
    tow_bar boolean,
    engine_size double precision,
    kilometers double precision,
    km_per_liter double precision,
    fuel_type fueltype,
    weight double precision,
    height double precision,
    length double precision,
    truck_cargo_capacity double precision,
    bus_seat_count integer,
    bed_count integer,
    toilet boolean,
    car_seat_count integer,
    business_cargo_capacity double precision,
    roll_cage boolean,
    isofix boolean
)
LANGUAGE sql
STABLE
AS $$
    SELECT  v.id, v.name, v.release_year, v.registration_number, v.base_price, v.tow_bar, v.engine_size,
            v.kilometers, v.km_per_liter, v.fuel_type,
            hv.weight, hv.height, hv.length,
            st.cargo_capacity,
            b.seat_count, b.bed_count, b.toilet,
            pc.seat_count,
            bpc.cargo_capacity, bpc.roll_cage,
            ppc.isofix
    FROM vehicles v
    LEFT JOIN heavy_vehicles hv ON v.id = hv.vehicle_id
    LEFT JOIN semi_trucks st ON hv.id = st.heavy_vehicle_id
    LEFT JOIN buses b ON hv.id = b.heavy_vehicle_id
    LEFT JOIN personal_cars pc ON v.id = pc.vehicle_id
    LEFT JOIN business_personal_cars bpc ON pc.id = bpc.car_id
    LEFT JOIN private_personal_cars ppc ON pc.id = ppc.car_id
    WHERE v.id = p_id;
$$;

CREATE OR REPLACE FUNCTION delete_vehicle(p_id integer)
RETURNS boolean
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM vehicles WHERE id = p_id;

    RETURN FOUND;
END;
$$;


CREATE ROLE auction_app LOGIN PASSWORD 'auction_app_dev_password';

GRANT USAGE ON SCHEMA public TO auction_app;

GRANT SELECT, INSERT, UPDATE, DELETE ON
    users,
    business_customers,
    private_customers,
    vehicles,
    heavy_vehicles,
    semi_trucks,
    buses,
    personal_cars,
    business_personal_cars,
    private_personal_cars,
    auctions,
    bids
TO auction_app;

GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO auction_app;

GRANT EXECUTE ON FUNCTION register_user(VARCHAR, VARCHAR, VARCHAR) TO auction_app;
GRANT EXECUTE ON FUNCTION get_user_by_username(VARCHAR) TO auction_app;

GRANT EXECUTE ON FUNCTION add_private_personal_car(VARCHAR, DOUBLE PRECISION, INTEGER, VARCHAR, NUMERIC, BOOLEAN, DOUBLE PRECISION, DOUBLE PRECISION, FuelType, INTEGER, BOOLEAN) TO auction_app;
GRANT EXECUTE ON FUNCTION add_business_personal_car(VARCHAR, DOUBLE PRECISION, INTEGER, VARCHAR, NUMERIC, BOOLEAN, DOUBLE PRECISION, DOUBLE PRECISION, FuelType, INTEGER, DOUBLE PRECISION, BOOLEAN) TO auction_app;
GRANT EXECUTE ON FUNCTION add_semi_truck(VARCHAR, DOUBLE PRECISION, INTEGER, VARCHAR, NUMERIC, BOOLEAN, DOUBLE PRECISION, DOUBLE PRECISION, FuelType, DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION) TO auction_app;
GRANT EXECUTE ON FUNCTION add_bus(VARCHAR, DOUBLE PRECISION, INTEGER, VARCHAR, NUMERIC, BOOLEAN, DOUBLE PRECISION, DOUBLE PRECISION, FuelType, DOUBLE PRECISION, DOUBLE PRECISION, DOUBLE PRECISION, INTEGER, INTEGER, BOOLEAN) TO auction_app;
GRANT EXECUTE ON FUNCTION get_vehicle_by_id(INTEGER) TO auction_app;
GRANT EXECUTE ON FUNCTION delete_vehicle(INTEGER) TO auction_app;
