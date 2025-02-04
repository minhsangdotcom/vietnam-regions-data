-- Auto-generated SQL script
-- Database: Mysql
CREATE TABLE administrative_unit (
    english_full_name VARCHAR(255),
    english_short_name VARCHAR(255),
    full_name VARCHAR(255),
    id INT NOT NULL PRIMARY KEY,
    short_name VARCHAR(255),
);

CREATE TABLE province (
    administrative_unit_id INT NOT NULL,
    code VARCHAR(255),
    custom_name VARCHAR(255),
    english_full_name VARCHAR(255),
    english_name VARCHAR(255),
    full_name VARCHAR(255),
    id CHAR(26) NOT NULL PRIMARY KEY,
    name VARCHAR(255),
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit(id),
);

CREATE TABLE district (
    administrative_unit_id INT NOT NULL,
    code VARCHAR(255),
    custom_name VARCHAR(255),
    english_full_name VARCHAR(255),
    english_name VARCHAR(255),
    full_name VARCHAR(255),
    id CHAR(26) NOT NULL PRIMARY KEY,
    name VARCHAR(255),
    province_code VARCHAR(255),
    province_id CHAR(26),
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit(id),
    FOREIGN KEY (province_id) REFERENCES province(id),
);

CREATE TABLE ward (
    administrative_unit_id INT NOT NULL,
    code VARCHAR(255),
    custom_name VARCHAR(255),
    district_code VARCHAR(255),
    district_id CHAR(26),
    english_full_name VARCHAR(255),
    english_name VARCHAR(255),
    full_name VARCHAR(255),
    id CHAR(26) NOT NULL PRIMARY KEY,
    name VARCHAR(255),
    FOREIGN KEY (administrative_unit_id) REFERENCES administrative_unit(id),
    FOREIGN KEY (district_id) REFERENCES district(id),
);

