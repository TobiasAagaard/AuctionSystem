CREATE SCHEMA IF NOT EXISTS auctions;

CREATE TABLE IF NOT EXISTS auctions.auctions (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title TEXT NOT NULL,
    description TEXT NOT NULL,
    price DECIMAL NOT NULL,
    status TEXT NOT NULL,
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    is_deleted BOOLEAN DEFAULT FALSE
);

CREATE TABLE IF NOT EXISTS auctions.vehicles (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    make TEXT NOT NULL,
    model TEXT NOT NULL,
    year INT NOT NULL,
    mileage INT NOT NULL,
    base_price DECIMAL NOT NULL,
    engine_size DOUBLE PRECISION NOT NULL,
    fuel_type TEXT NOT NULL,
    vehicle_type TEXT NOT NULL
        CHECK (vehicle_type IN ('personal_car', 'business_car')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    auction_id INT NOT NULL REFERENCES auctions.auctions(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS auctions.personal_cars_specs (
    vehicle_id INT PRIMARY KEY REFERENCES auctions.vehicles(id) ON DELETE CASCADE,
    seat_count INT NOT NULL,
    isofix BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS auctions.business_cars_specs (
    vehicle_id INT PRIMARY KEY REFERENCES auctions.vehicles(id) ON DELETE CASCADE,
    cargo_volume DECIMAL NOT NULL,
    max_payload DECIMAL NOT NULL
);