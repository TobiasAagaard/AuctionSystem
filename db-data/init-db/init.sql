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
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE bids (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    auction_id INT NOT NULL REFERENCES auctions(id),
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


-- D3: the application connects as a least-privileged role that owns nothing
-- and therefore cannot DROP, ALTER or otherwise change the schema.

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



CREATE OR REPLACE FUNCTION add_vehicle(
    p_name VARCHAR(255),
    p_kilometers double precision,
    p_release_year int,
    p_registration_number VARCHAR(20),
    p_base_price DECIMAL(18,2),
    p_tow_bar boolean,
    p_engine_size double precision,
    p_km_per_liter double precision,
    p_fuel_type FuelType
)
RETURNS INT
LANGUAGE plpgsql
AS $$
DECLARE
    new_vehicle_id INT;
BEGIN
    INSERT INTO vehicles (
        name, kilometers, release_year, registration_number, base_price,
        tow_bar, engine_size, km_per_liter, fuel_type
    )
    VALUES (
        p_name, p_kilometers, p_release_year, p_registration_number,
        p_base_price, p_tow_bar, p_engine_size, p_km_per_liter, p_fuel_type
    )
    RETURNING id INTO new_vehicle_id;

    RETURN new_vehicle_id;
END;
$$;

CREATE OR REPLACE FUNCTION add_semi_truck(
    -- Vehicle parameters
    p_name VARCHAR(255),
    p_kilometers double precision,
    p_release_year int,
    p_registration_number VARCHAR(20),
    p_base_price DECIMAL(18,2),
    p_tow_bar boolean,
    p_engine_size double precision,
    p_km_per_liter double precision,
    p_fuel_type FuelType,

    -- Heavy vehicle parameters
    p_weight double precision,
    p_height double precision,
    p_length double precision,

    -- Semi truck parameters
    p_cargo_capacity double precision
)
RETURNS TABLE (
    vehicle_id INT,
    heavy_vehicle_id INT,
    semi_truck_id INT
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_now timestamptz := now();
BEGIN
    vehicle_id := add_vehicle(
        p_name, p_kilometers, p_release_year, p_registration_number,
        p_base_price, p_tow_bar, p_engine_size, p_km_per_liter, p_fuel_type
    );

    INSERT INTO heavy_vehicles (vehicle_id, weight, height, length, created_at, updated_at)
    VALUES (vehicle_id, p_weight, p_height, p_length, v_now, v_now)
    RETURNING id INTO heavy_vehicle_id;

    INSERT INTO semi_trucks (heavy_vehicle_id, cargo_capacity, created_at, updated_at)
    VALUES (heavy_vehicle_id, p_cargo_capacity, v_now, v_now);

    semi_truck_id := heavy_vehicle_id;

    RETURN QUERY
    SELECT vehicle_id, heavy_vehicle_id, semi_truck_id;
END;
$$;