CREATE SCHEMA IF NOT EXISTS vehicles;

CREATE TABLE vehicles.vehicles (
    id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    vin VARCHAR(17) UNIQUE NOT NULL,
    make TEXT NOT NULL,
    model TEXT NOT NULL,
    year INT NOT NULL,
    mileage INT NOT NULL,
    base_price DECIMAL(18,2) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    type TEXT NOT NULL
        CHECK ( type IN('Car', 'Bus', 'SemiTruck') )
);

CREATE TABLE vehicles.car_specs (
    vehicle_id INT PRIMARY KEY REFERENCES vehicles.vehicles(id) ON DELETE CASCADE,
    seat_count INT NOT NULL,
    isofix BOOLEAN NOT NULL
);

CREATE TABLE vehicles.bus_specs (
    vehicle_id INT PRIMARY KEY REFERENCES vehicles.vehicles(id) ON DELETE CASCADE,
    seat_count INT NOT NULL,
    bed_count INT NOT NULL,
    toilet BOOLEAN NOT NULL
);

CREATE TABLE vehicles.semi_truck_specs (
    vehicle_id INT PRIMARY KEY REFERENCES vehicles.vehicles(id) ON DELETE CASCADE,
    weight DECIMAL(10,2) NOT NULL,
    height DECIMAL(10,2) NOT NULL,
    length DECIMAL(10,2) NOT NULL
);