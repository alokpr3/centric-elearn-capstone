CREATE TABLE regions (
  id INT NOT NULL AUTO_INCREMENT,
  code VARCHAR(10) NOT NULL,
  name VARCHAR(60) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uq_regions_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE products (
  id INT NOT NULL AUTO_INCREMENT,
  code VARCHAR(20) NOT NULL,
  name VARCHAR(80) NOT NULL,
  line VARCHAR(20) NOT NULL,
  base_premium DECIMAL(12,2) NOT NULL,
  default_excess DECIMAL(12,2) NOT NULL,
  active TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (id),
  UNIQUE KEY uq_products_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE handler_grades (
  id INT NOT NULL AUTO_INCREMENT,
  code VARCHAR(10) NOT NULL,
  name VARCHAR(60) NOT NULL,
  approval_limit DECIMAL(12,2) NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uq_handler_grades_code (code)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE users (
  id INT NOT NULL AUTO_INCREMENT,
  email VARCHAR(120) NOT NULL,
  password_hash VARCHAR(100) NOT NULL,
  role VARCHAR(20) NOT NULL,
  display_name VARCHAR(80) NOT NULL,
  active TINYINT(1) NOT NULL DEFAULT 1,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uq_users_email (email)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE handlers (
  id INT NOT NULL AUTO_INCREMENT,
  user_id INT NOT NULL,
  name VARCHAR(80) NOT NULL,
  email VARCHAR(120) NOT NULL,
  grade_id INT NOT NULL,
  region_id INT NOT NULL,
  max_open_claims INT NOT NULL,
  active TINYINT(1) NOT NULL DEFAULT 1,
  PRIMARY KEY (id),
  KEY ix_handlers_grade (grade_id),
  KEY ix_handlers_region (region_id),
  CONSTRAINT fk_handlers_user FOREIGN KEY (user_id) REFERENCES users (id),
  CONSTRAINT fk_handlers_grade FOREIGN KEY (grade_id) REFERENCES handler_grades (id),
  CONSTRAINT fk_handlers_region FOREIGN KEY (region_id) REFERENCES regions (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE policyholders (
  id INT NOT NULL AUTO_INCREMENT,
  full_name VARCHAR(100) NOT NULL,
  email VARCHAR(120) NOT NULL,
  phone VARCHAR(30) NULL,
  address_line VARCHAR(120) NOT NULL,
  city VARCHAR(60) NOT NULL,
  postcode VARCHAR(12) NOT NULL,
  date_of_birth DATE NOT NULL,
  region_id INT NOT NULL,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  KEY ix_policyholders_region (region_id),
  CONSTRAINT fk_policyholders_region FOREIGN KEY (region_id) REFERENCES regions (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE policies (
  id INT NOT NULL AUTO_INCREMENT,
  policy_number VARCHAR(20) NOT NULL,
  policyholder_id INT NOT NULL,
  product_id INT NOT NULL,
  start_date DATE NOT NULL,
  end_date DATE NOT NULL,
  annual_premium DECIMAL(12,2) NOT NULL,
  sum_insured DECIMAL(12,2) NOT NULL,
  status VARCHAR(20) NOT NULL,
  region_id INT NOT NULL,
  no_claims_years INT NOT NULL DEFAULT 0,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uq_policies_number (policy_number),
  KEY ix_policies_holder (policyholder_id),
  KEY ix_policies_product (product_id),
  CONSTRAINT fk_policies_holder FOREIGN KEY (policyholder_id) REFERENCES policyholders (id),
  CONSTRAINT fk_policies_product FOREIGN KEY (product_id) REFERENCES products (id),
  CONSTRAINT fk_policies_region FOREIGN KEY (region_id) REFERENCES regions (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE claims (
  id INT NOT NULL AUTO_INCREMENT,
  claim_number VARCHAR(20) NOT NULL,
  policy_id INT NOT NULL,
  handler_id INT NULL,
  claim_type VARCHAR(40) NOT NULL,
  status VARCHAR(20) NOT NULL,
  loss_date DATE NOT NULL,
  reported_date DATE NOT NULL,
  description VARCHAR(1000) NOT NULL,
  estimated_amount DECIMAL(12,2) NOT NULL,
  approved_amount DECIMAL(12,2) NULL,
  paid_amount DECIMAL(12,2) NULL,
  excess DECIMAL(12,2) NOT NULL,
  region_id INT NOT NULL,
  reopened_count INT NOT NULL DEFAULT 0,
  closed_date DATE NULL,
  external_ref VARCHAR(40) NULL,
  created_at DATETIME NOT NULL,
  updated_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  UNIQUE KEY uq_claims_number (claim_number),
  KEY ix_claims_policy (policy_id),
  KEY ix_claims_handler (handler_id),
  KEY ix_claims_status (status),
  KEY ix_claims_region (region_id),
  CONSTRAINT fk_claims_policy FOREIGN KEY (policy_id) REFERENCES policies (id),
  CONSTRAINT fk_claims_handler FOREIGN KEY (handler_id) REFERENCES handlers (id),
  CONSTRAINT fk_claims_region FOREIGN KEY (region_id) REFERENCES regions (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE claim_notes (
  id INT NOT NULL AUTO_INCREMENT,
  claim_id INT NOT NULL,
  author_user_id INT NOT NULL,
  note_type VARCHAR(20) NOT NULL,
  body VARCHAR(500) NOT NULL,
  created_at DATETIME NOT NULL,
  PRIMARY KEY (id),
  KEY ix_claim_notes_claim (claim_id),
  CONSTRAINT fk_claim_notes_claim FOREIGN KEY (claim_id) REFERENCES claims (id),
  CONSTRAINT fk_claim_notes_author FOREIGN KEY (author_user_id) REFERENCES users (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE claim_status_history (
  id INT NOT NULL AUTO_INCREMENT,
  claim_id INT NOT NULL,
  from_status VARCHAR(20) NULL,
  to_status VARCHAR(20) NOT NULL,
  changed_by_user_id INT NOT NULL,
  changed_at DATETIME NOT NULL,
  reason VARCHAR(300) NULL,
  PRIMARY KEY (id),
  KEY ix_claim_history_claim (claim_id),
  KEY ix_claim_history_to (to_status),
  CONSTRAINT fk_claim_history_claim FOREIGN KEY (claim_id) REFERENCES claims (id),
  CONSTRAINT fk_claim_history_user FOREIGN KEY (changed_by_user_id) REFERENCES users (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE import_batches (
  id INT NOT NULL AUTO_INCREMENT,
  file_name VARCHAR(120) NOT NULL,
  source VARCHAR(80) NOT NULL,
  total_rows INT NOT NULL,
  created_rows INT NOT NULL,
  updated_rows INT NOT NULL,
  rejected_rows INT NOT NULL,
  created_at DATETIME NOT NULL,
  created_by_user_id INT NOT NULL,
  PRIMARY KEY (id),
  CONSTRAINT fk_import_batches_user FOREIGN KEY (created_by_user_id) REFERENCES users (id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
