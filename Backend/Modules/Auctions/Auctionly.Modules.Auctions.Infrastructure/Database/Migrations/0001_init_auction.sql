CREATE SCHEMA IF NOT EXISTS auctions;

CREATE TABLE IF NOT EXISTS auctions.auctions (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title TEXT NOT NULL,
    description TEXT NOT NULL,
    price DECIMAL (18, 2) NOT NULL,
    currency TEXT NOT NULL,
        CHECK (currency IN ('USD', 'EUR', 'DKK')),
    status TEXT NOT NULL,
        CHECK (status IN ('Draft', 'Active', 'Sold', 'Cancelled')),
    start_time TIMESTAMP NOT NULL,
    end_time TIMESTAMP NOT NULL,
        CHECK (end_time > start_time),
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
    purchase_price DECIMAL (18, 2) NOT NULL,
    currency TEXT NOT NULL,
        CHECK (currency IN ('USD', 'EUR', 'DKK')),
    engine_size DOUBLE PRECISION NOT NULL,
    fuel_type TEXT NOT NULL
        CHECK (fuel_type IN ('Petrol', 'Diesel', 'Electric', 'Hybrid')),
    vehicle_type TEXT NOT NULL
        CHECK (vehicle_type IN ('PersonalCar', 'BusinessCar')),
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    auction_id INT NOT NULL UNIQUE REFERENCES auctions.auctions(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS auctions.personal_cars (
    vehicle_id INT PRIMARY KEY REFERENCES auctions.vehicles(id) ON DELETE CASCADE,
    seat_count INT NOT NULL,
    isofix BOOLEAN NOT NULL
);

CREATE TABLE IF NOT EXISTS auctions.business_cars (
    vehicle_id INT PRIMARY KEY REFERENCES auctions.vehicles(id) ON DELETE CASCADE,
    cargo_volume DOUBLE PRECISION NOT NULL,
    max_payload DOUBLE PRECISION NOT NULL
);

CREATE TABLE IF NOT EXISTS auctions.bids (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    auction_id INT NOT NULL REFERENCES auctions.auctions(id) ON DELETE CASCADE,
    amount DECIMAL(18, 2) NOT NULL,
    currency TEXT NOT NULL,
        CHECK (currency IN ('USD', 'EUR', 'DKK')),
    created_at timestamptz DEFAULT NOW()
);

CREATE TABLE IF NOT EXISTS auctions.auction_bid_history (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    auction_id INT NOT NULL REFERENCES auctions.auctions(id) ON DELETE CASCADE,
    bid_id INT NOT NULL REFERENCES auctions.bids(id) ON DELETE CASCADE
);