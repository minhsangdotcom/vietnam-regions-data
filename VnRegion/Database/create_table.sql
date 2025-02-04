-- Auto-generated SQL script
-- Database: SqlServer
CREATE DATABASE vietnamese_region_data;
USE vietnamese_region_data;

CREATE TABLE administrative_unit (
    id INT NOT NULL PRIMARY KEY,
    full_name NVARCHAR(255),
    english_full_name VARCHAR(255),
    short_name VARCHAR(255),
    english_short_name VARCHAR(255),
);

CREATE TABLE province (
    id CHAR(26) NOT NULL PRIMARY KEY,
    code VARCHAR(255),
    name NVARCHAR(255),
    english_name VARCHAR(255),
    full_name NVARCHAR(255),
    english_full_name VARCHAR(255),
    custom_name NVARCHAR(255),
    administrative_unit_id INT NOT NULL,
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit(id),
);

CREATE TABLE district (
    province_code VARCHAR(255),
    province_id CHAR(26),
    id CHAR(26) NOT NULL PRIMARY KEY,
    code VARCHAR(255),
    name NVARCHAR(255),
    english_name VARCHAR(255),
    full_name NVARCHAR(255),
    english_full_name VARCHAR(255),
    custom_name NVARCHAR(255),
    administrative_unit_id INT NOT NULL,
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit(id),
    FOREIGN KEY (province_id) REFERENCES province(id),
);

CREATE TABLE ward (
    district_code VARCHAR(255),
    district_id CHAR(26),
    id CHAR(26) NOT NULL PRIMARY KEY,
    code VARCHAR(255),
    name NVARCHAR(255),
    english_name VARCHAR(255),
    full_name NVARCHAR(255),
    english_full_name VARCHAR(255),
    custom_name NVARCHAR(255),
    administrative_unit_id INT NOT NULL,
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit(id),
    FOREIGN KEY (district_id) REFERENCES district(id),
);

