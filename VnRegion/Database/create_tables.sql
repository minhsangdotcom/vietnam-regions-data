-- SQL script to create Province, District, Ward, and AdministrativeUnit tables in snake_case.
-- Compatible with SQL Server, MySQL, PostgreSQL, and Oracle.

-- Create AdministrativeUnit table
CREATE TABLE administrative_unit (
    id INT NOT NULL PRIMARY KEY,
    full_name VARCHAR(255),
    english_full_name VARCHAR(255),
    short_name VARCHAR(255),
    english_short_name VARCHAR(255),
    created_at TIMESTAMP
);

-- Create Province table
CREATE TABLE province (
    id CHAR(26) NOT NULL PRIMARY KEY,
    code VARCHAR(255),
    name VARCHAR(255),
    english_name VARCHAR(255),
    full_name VARCHAR(255),
    english_full_name VARCHAR(255),
    custom_name VARCHAR(255),
    administrative_unit_id INT NOT NULL,
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit (id)
);

-- Create District table
CREATE TABLE district (
    id CHAR(26) NOT NULL PRIMARY KEY,
    code VARCHAR(255),
    name VARCHAR(255),
    english_name VARCHAR(255),
    full_name VARCHAR(255),
    english_full_name VARCHAR(255),
    custom_name VARCHAR(255),
    administrative_unit_id INT NOT NULL,
    province_code VARCHAR(255),
    province_id CHAR(26),
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit (id),
    FOREIGN KEY (province_id) REFERENCES province (id)
);

-- Create Ward table
CREATE TABLE ward (
    id CHAR(26) NOT NULL PRIMARY KEY,
    code VARCHAR(255),
    name VARCHAR(255),
    english_name VARCHAR(255),
    full_name VARCHAR(255),
    english_full_name VARCHAR(255),
    custom_name VARCHAR(255),
    administrative_unit_id INT NOT NULL,
    district_code VARCHAR(255),
    district_id CHAR(26),
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit (id),
    FOREIGN KEY (district_id) REFERENCES district (id)
);
