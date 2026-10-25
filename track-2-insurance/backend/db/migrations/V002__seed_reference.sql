-- Reference data
INSERT INTO regions (id, code, name) VALUES
(1, 'NTH', 'Scotland & North'),
(2, 'MID', 'Midlands'),
(3, 'LSE', 'London & South East'),
(4, 'SWW', 'South West & Wales');

INSERT INTO products (id, code, name, line, base_premium, default_excess, active) VALUES
(1, 'HOME-BAS', 'Home Basic', 'Home', 380, 250, 1),
(2, 'HOME-STD', 'Home Standard', 'Home', 520, 250, 1),
(3, 'HOME-PRM', 'Home Premier', 'Home', 790, 500, 1),
(4, 'HOME-LLD', 'Landlord Home', 'Home', 610, 350, 1),
(5, 'HOME-CNT', 'Contents Only', 'Home', 210, 150, 1),
(6, 'MOT-TPO', 'Motor Third Party Only', 'Motor', 540, 250, 1),
(7, 'MOT-TPFT', 'Motor Third Party Fire & Theft', 'Motor', 690, 250, 1),
(8, 'MOT-COMP', 'Motor Comprehensive', 'Motor', 980, 300, 1),
(9, 'MOT-CLS', 'Classic Car', 'Motor', 460, 300, 1),
(10, 'MOT-VAN', 'Motor Van', 'Motor', 1150, 400, 1);

INSERT INTO handler_grades (id, code, name, approval_limit) VALUES
(1, 'G1', 'Claims Assistant', 2000),
(2, 'G2', 'Claims Handler', 10000),
(3, 'G3', 'Senior Handler', 50000),
(4, 'G4', 'Technical Lead', 250000);

INSERT INTO users (id, email, password_hash, role, display_name, active, created_at) VALUES
(1, 'admin@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Admin', 'Site Administrator', 1, '2024-01-02 09:00:00'),
(2, 'priya.raman@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Manager', 'Priya Raman', 1, '2024-01-02 09:00:00'),
(3, 'daniel.okafor@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Daniel Okafor', 1, '2024-01-02 09:00:00'),
(4, 'rachel.bennett@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Rachel Bennett', 1, '2024-01-02 09:00:00'),
(5, 'tom.hargreaves@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Tom Hargreaves', 1, '2024-01-02 09:00:00'),
(6, 'sophie.lang@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Sophie Lang', 1, '2024-01-02 09:00:00'),
(7, 'alasdair.munro@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Alasdair Munro', 1, '2024-01-02 09:00:00'),
(8, 'jenny.craig@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Jenny Craig', 1, '2024-01-02 09:00:00'),
(9, 'kamal.hussain@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Kamal Hussain', 1, '2024-01-02 09:00:00'),
(10, 'helen.pryce@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Helen Pryce', 1, '2024-01-02 09:00:00'),
(11, 'marcus.reid@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Marcus Reid', 1, '2024-01-02 09:00:00'),
(12, 'zoe.fletcher@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Zoe Fletcher', 1, '2024-01-02 09:00:00'),
(13, 'ioan.griffiths@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Ioan Griffiths', 1, '2024-01-02 09:00:00'),
(14, 'beth.tanner@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'Handler', 'Beth Tanner', 1, '2024-01-02 09:00:00'),
(15, 'audit.readonly@harbourline.example', 'cbbbdcfe1d5dfc51ce7097c68c191966a45789bad60d830d1bf022594384150b', 'ReadOnly', 'Internal Audit', 1, '2024-01-02 09:00:00');

INSERT INTO handlers (id, user_id, name, email, grade_id, region_id, max_open_claims, active) VALUES
(1, 3, 'Daniel Okafor', 'daniel.okafor@harbourline.example', 4, 3, 25, 1),
(2, 4, 'Rachel Bennett', 'rachel.bennett@harbourline.example', 3, 3, 20, 1),
(3, 5, 'Tom Hargreaves', 'tom.hargreaves@harbourline.example', 2, 3, 7, 1),
(4, 6, 'Sophie Lang', 'sophie.lang@harbourline.example', 1, 3, 8, 1),
(5, 7, 'Alasdair Munro', 'alasdair.munro@harbourline.example', 3, 1, 20, 1),
(6, 8, 'Jenny Craig', 'jenny.craig@harbourline.example', 2, 1, 15, 1),
(7, 9, 'Kamal Hussain', 'kamal.hussain@harbourline.example', 2, 1, 15, 1),
(8, 10, 'Helen Pryce', 'helen.pryce@harbourline.example', 3, 2, 20, 1),
(9, 11, 'Marcus Reid', 'marcus.reid@harbourline.example', 2, 2, 15, 1),
(10, 12, 'Zoe Fletcher', 'zoe.fletcher@harbourline.example', 2, 2, 15, 1),
(11, 13, 'Ioan Griffiths', 'ioan.griffiths@harbourline.example', 1, 4, 8, 1),
(12, 14, 'Beth Tanner', 'beth.tanner@harbourline.example', 1, 4, 12, 1);

