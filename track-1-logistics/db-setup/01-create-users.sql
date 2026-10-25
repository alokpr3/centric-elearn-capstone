-- Run once as the MySQL root user:  mysql -h 127.0.0.1 -P 3306 -u root -p < db-setup/01-create-users.sql
CREATE DATABASE IF NOT EXISTS northgate CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci;
CREATE USER IF NOT EXISTS 'northgate_app'@'%' IDENTIFIED BY 'Ngf-App#2026';
GRANT ALL PRIVILEGES ON northgate.* TO 'northgate_app'@'%';
FLUSH PRIVILEGES;
