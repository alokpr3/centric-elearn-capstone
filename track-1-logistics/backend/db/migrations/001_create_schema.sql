CREATE TABLE depots (
  id INT NOT NULL AUTO_INCREMENT,
  code VARCHAR(6) NOT NULL,
  name VARCHAR(80) NOT NULL,
  region_code VARCHAR(4) NOT NULL,
  city VARCHAR(60) NOT NULL,
  daily_capacity INT NOT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY ux_depots_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE customers (
  id INT NOT NULL AUTO_INCREMENT,
  account_no VARCHAR(12) NOT NULL,
  name VARCHAR(120) NOT NULL,
  contact_name VARCHAR(80) NOT NULL,
  email VARCHAR(120) NOT NULL,
  phone VARCHAR(30) NOT NULL,
  address_line VARCHAR(160) NOT NULL,
  city VARCHAR(60) NOT NULL,
  postcode VARCHAR(10) NOT NULL,
  region_code VARCHAR(4) NOT NULL,
  credit_limit DECIMAL(10,2) NOT NULL DEFAULT 0,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY ux_customers_account_no (account_no)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE users (
  id INT NOT NULL AUTO_INCREMENT,
  username VARCHAR(50) NOT NULL,
  email VARCHAR(120) NOT NULL,
  password_hash VARCHAR(200) NOT NULL,
  role VARCHAR(20) NOT NULL,
  region_code VARCHAR(4) NULL,
  depot_id INT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL,
  last_login_at DATETIME NULL,
  PRIMARY KEY (id),
  UNIQUE KEY ux_users_username (username)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE drivers (
  id INT NOT NULL AUTO_INCREMENT,
  depot_id INT NOT NULL,
  name VARCHAR(80) NOT NULL,
  phone VARCHAR(30) NOT NULL,
  licence_no VARCHAR(24) NOT NULL,
  vehicle_reg VARCHAR(10) NOT NULL,
  max_stops INT NOT NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  KEY ix_drivers_depot (depot_id),
  CONSTRAINT fk_drivers_depot FOREIGN KEY (depot_id) REFERENCES depots (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE consignments (
  id INT NOT NULL AUTO_INCREMENT,
  reference VARCHAR(16) NOT NULL,
  customer_id INT NOT NULL,
  origin_depot_id INT NOT NULL,
  dest_depot_id INT NOT NULL,
  region_code VARCHAR(4) NOT NULL,
  service_level VARCHAR(16) NOT NULL,
  status VARCHAR(20) NOT NULL,
  parcels INT NOT NULL,
  weight_kg DECIMAL(8,2) NOT NULL,
  recipient_name VARCHAR(100) NOT NULL,
  recipient_phone VARCHAR(30) NOT NULL,
  delivery_address VARCHAR(200) NOT NULL,
  delivery_postcode VARCHAR(10) NOT NULL,
  booked_at DATETIME NOT NULL,
  promised_by DATETIME NOT NULL,
  delivered_at DATETIME NULL,
  cancelled_at DATETIME NULL,
  driver_id INT NULL,
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  KEY ix_consignments_reference (reference),
  KEY ix_consignments_status (status),
  KEY ix_consignments_promised_by (promised_by),
  KEY ix_consignments_customer (customer_id),
  KEY ix_consignments_origin (origin_depot_id),
  KEY ix_consignments_dest (dest_depot_id),
  KEY ix_consignments_driver (driver_id),
  CONSTRAINT fk_consignments_customer FOREIGN KEY (customer_id) REFERENCES customers (id),
  CONSTRAINT fk_consignments_origin FOREIGN KEY (origin_depot_id) REFERENCES depots (id),
  CONSTRAINT fk_consignments_dest FOREIGN KEY (dest_depot_id) REFERENCES depots (id),
  CONSTRAINT fk_consignments_driver FOREIGN KEY (driver_id) REFERENCES drivers (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE consignment_notes (
  id INT NOT NULL AUTO_INCREMENT,
  consignment_id INT NOT NULL,
  author_user_id INT NOT NULL,
  note_type VARCHAR(12) NOT NULL,
  note VARCHAR(255) NOT NULL,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  KEY ix_notes_consignment (consignment_id),
  CONSTRAINT fk_notes_consignment FOREIGN KEY (consignment_id) REFERENCES consignments (id),
  CONSTRAINT fk_notes_author FOREIGN KEY (author_user_id) REFERENCES users (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE consignment_events (
  id INT NOT NULL AUTO_INCREMENT,
  consignment_id INT NOT NULL,
  event_type VARCHAR(24) NOT NULL,
  depot_id INT NOT NULL,
  occurred_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  KEY ix_events_consignment (consignment_id, occurred_at),
  CONSTRAINT fk_events_consignment FOREIGN KEY (consignment_id) REFERENCES consignments (id),
  CONSTRAINT fk_events_depot FOREIGN KEY (depot_id) REFERENCES depots (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE import_batches (
  id INT NOT NULL AUTO_INCREMENT,
  file_name VARCHAR(200) NOT NULL,
  total_rows INT NOT NULL,
  imported_rows INT NOT NULL,
  skipped_rows INT NOT NULL,
  summary TEXT NULL,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
