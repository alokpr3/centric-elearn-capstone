CREATE TABLE hotels (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(8) NOT NULL,
  name VARCHAR(120) NOT NULL,
  city VARCHAR(80) NOT NULL,
  region VARCHAR(20) NOT NULL,
  address_line VARCHAR(200) NOT NULL,
  phone VARCHAR(30) NOT NULL,
  active TINYINT(1) NOT NULL DEFAULT 1,
  UNIQUE KEY ux_hotels_code (code)
);

CREATE TABLE room_types (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(8) NOT NULL,
  name VARCHAR(80) NOT NULL,
  max_occupancy INT NOT NULL,
  base_rate DECIMAL(10,2) NOT NULL,
  UNIQUE KEY ux_room_types_code (code)
);

CREATE TABLE rate_plans (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  code VARCHAR(8) NOT NULL,
  name VARCHAR(80) NOT NULL,
  discount_pct DECIMAL(5,2) NOT NULL DEFAULT 0,
  requires_prepayment TINYINT(1) NOT NULL DEFAULT 0,
  UNIQUE KEY ux_rate_plans_code (code)
);

CREATE TABLE room_rates (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  hotel_id INT NOT NULL,
  room_type_id INT NOT NULL,
  rate_plan_id INT NOT NULL,
  valid_from DATE NOT NULL,
  valid_to DATE NOT NULL,
  nightly_rate DECIMAL(10,2) NOT NULL,
  KEY ix_room_rates_lookup (hotel_id, room_type_id, rate_plan_id),
  CONSTRAINT fk_room_rates_hotel FOREIGN KEY (hotel_id) REFERENCES hotels (id),
  CONSTRAINT fk_room_rates_type FOREIGN KEY (room_type_id) REFERENCES room_types (id),
  CONSTRAINT fk_room_rates_plan FOREIGN KEY (rate_plan_id) REFERENCES rate_plans (id)
);

CREATE TABLE users (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  email VARCHAR(200) NOT NULL,
  display_name VARCHAR(120) NOT NULL,
  role VARCHAR(20) NOT NULL,
  hotel_id INT NULL,
  password_hash VARCHAR(200) NOT NULL,
  active TINYINT(1) NOT NULL DEFAULT 1,
  phone VARCHAR(30) NULL,
  last_login_at DATETIME NULL,
  UNIQUE KEY ux_users_email (email),
  CONSTRAINT fk_users_hotel FOREIGN KEY (hotel_id) REFERENCES hotels (id)
);

CREATE TABLE rooms (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  hotel_id INT NOT NULL,
  room_type_id INT NOT NULL,
  room_number VARCHAR(10) NOT NULL,
  floor INT NOT NULL,
  status VARCHAR(20) NOT NULL DEFAULT 'Clean',
  notes VARCHAR(255) NULL,
  UNIQUE KEY ux_rooms_hotel_number (hotel_id, room_number),
  CONSTRAINT fk_rooms_hotel FOREIGN KEY (hotel_id) REFERENCES hotels (id),
  CONSTRAINT fk_rooms_type FOREIGN KEY (room_type_id) REFERENCES room_types (id)
);

CREATE TABLE room_status_history (
  id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  room_id INT NOT NULL,
  from_status VARCHAR(20) NULL,
  to_status VARCHAR(20) NOT NULL,
  changed_by_user_id INT NULL,
  changed_at DATETIME NOT NULL,
  KEY ix_room_status_history_room (room_id, changed_at),
  CONSTRAINT fk_rsh_room FOREIGN KEY (room_id) REFERENCES rooms (id)
);

CREATE TABLE guests (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  first_name VARCHAR(80) NOT NULL,
  last_name VARCHAR(80) NOT NULL,
  email VARCHAR(200) NOT NULL,
  phone VARCHAR(30) NOT NULL,
  address_line VARCHAR(200) NOT NULL,
  city VARCHAR(80) NOT NULL,
  postcode VARCHAR(20) NOT NULL,
  country VARCHAR(2) NOT NULL,
  region VARCHAR(20) NOT NULL,
  loyalty_tier VARCHAR(20) NOT NULL DEFAULT 'None',
  created_at DATETIME NOT NULL,
  KEY ix_guests_email (email),
  KEY ix_guests_last_name (last_name)
);

CREATE TABLE bookings (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  reference VARCHAR(20) NOT NULL,
  external_ref VARCHAR(40) NULL,
  channel VARCHAR(30) NOT NULL,
  hotel_id INT NOT NULL,
  guest_id INT NOT NULL,
  room_type_id INT NOT NULL,
  room_id INT NULL,
  rate_plan_id INT NOT NULL,
  check_in DATE NOT NULL,
  check_out DATE NOT NULL,
  adults INT NOT NULL,
  status VARCHAR(20) NOT NULL,
  total_amount DECIMAL(10,2) NOT NULL,
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  UNIQUE KEY ux_bookings_reference (reference),
  KEY ix_bookings_hotel_dates (hotel_id, check_in),
  KEY ix_bookings_guest (guest_id),
  KEY ix_bookings_room (room_id),
  CONSTRAINT fk_bookings_hotel FOREIGN KEY (hotel_id) REFERENCES hotels (id),
  CONSTRAINT fk_bookings_guest FOREIGN KEY (guest_id) REFERENCES guests (id),
  CONSTRAINT fk_bookings_type FOREIGN KEY (room_type_id) REFERENCES room_types (id),
  CONSTRAINT fk_bookings_room FOREIGN KEY (room_id) REFERENCES rooms (id),
  CONSTRAINT fk_bookings_plan FOREIGN KEY (rate_plan_id) REFERENCES rate_plans (id)
);

CREATE TABLE booking_notes (
  id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,
  booking_id INT NOT NULL,
  author_user_id INT NULL,
  note VARCHAR(255) NOT NULL,
  created_at DATETIME NOT NULL,
  KEY ix_booking_notes_booking (booking_id),
  CONSTRAINT fk_booking_notes_booking FOREIGN KEY (booking_id) REFERENCES bookings (id)
);
