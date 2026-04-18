IF NOT EXISTS (
    SELECT name
    FROM sys.databases
    WHERE name = 'DutyFreeShops_Test'
)
BEGIN
    CREATE DATABASE DutyFreeShops_Test;
END;
GO

USE DutyFreeShops_Test;
GO

DROP TABLE IF EXISTS CartItem
DROP TABLE IF EXISTS Reservation
DROP TABLE IF EXISTS Cart
DROP TABLE IF EXISTS Item
DROP TABLE IF EXISTS Shop
DROP TABLE IF EXISTS Manager
DROP TABLE IF EXISTS Client
DROP TABLE IF EXISTS Ticket
GO

CREATE TABLE Manager (
    manager_id  INT           IDENTITY(1,1) PRIMARY KEY,
    name        VARCHAR(50),
    email       VARCHAR(50),
    phone       VARCHAR(15)
);
GO

CREATE TABLE Shop (
    shop_id     INT           IDENTITY(1,1) PRIMARY KEY,
    type        VARCHAR(50)   NOT NULL,
    name        VARCHAR(50)   NOT NULL,
    manager_id  INT           FOREIGN KEY REFERENCES Manager(manager_id)
);
GO

CREATE TABLE Item (
    item_id     INT           IDENTITY(1,1) PRIMARY KEY,
    shop_id     INT           FOREIGN KEY REFERENCES Shop(shop_id) ON DELETE CASCADE,
    name        VARCHAR(100),
    description VARCHAR(500),
    price       FLOAT,
    stock       INT,
    img         VARCHAR(300)
);
GO

CREATE TABLE Client (
    client_id     INT         IDENTITY(1,1) PRIMARY KEY,
    name          VARCHAR(50),
    date_of_birth DATE
);
GO

CREATE TABLE Cart (
    cart_id    INT            IDENTITY(1,1) PRIMARY KEY,
    client_id  INT            FOREIGN KEY REFERENCES Client(client_id),
    status     VARCHAR(50)
);
GO

CREATE TABLE CartItem (
    cart_item_id  INT         IDENTITY(1,1) PRIMARY KEY,
    cart_id       INT         FOREIGN KEY REFERENCES Cart(cart_id),
    item_id       INT         FOREIGN KEY REFERENCES Item(item_id),
    quantity      INT
);
GO

CREATE TABLE Reservation (
    reservation_id    INT     IDENTITY(1,1) PRIMARY KEY,
    cart_id           INT     FOREIGN KEY REFERENCES Cart(cart_id),
    reservation_date  DATE,
    time_slot         TIME,
    active            BIT     NOT NULL DEFAULT 1
);
GO

CREATE TABLE Ticket (
    ticket_id    INT          IDENTITY(1,1) PRIMARY KEY,
    category     VARCHAR(50),
    subcategory  VARCHAR(50)
);
GO

-- ----------------------------
-- Test Data
-- ----------------------------

INSERT INTO Manager (name, email, phone) VALUES
('Marcel', 'marcel@gmail.com', '4074593789'),
('Sofia',  'sofia@gmail.com',  '4078812345');

INSERT INTO Shop (type, name, manager_id) VALUES
('Food & Beverage',   'Sky Bites',      1),
('Coffee Shop',       'Runway Cafe',    1),
('Luxury Goods',      'Elite Boutique', 2),
('Travel Essentials', 'FlySmart Store', 2);

INSERT INTO Item (shop_id, name, description, price, stock, img) VALUES
(1, 'Chicken Sandwich', 'Fresh sandwich',         12.5,  98, 'https://images.unsplash.com/photo-1568901346375-23c9450c58cd'),
(1, 'Caesar Salad',     'Healthy salad',            8.0,  77, 'https://images.unsplash.com/photo-1551248429-40975aa4de74'),
(1, 'Orange Juice',     'Fresh juice',               5.5,  60, 'https://images.unsplash.com/photo-1621506289937-a8e4df240d0b'),
(2, 'Espresso',         'Strong coffee',             3.5, 200, 'https://images.unsplash.com/photo-1511920170033-f8396924c348'),
(2, 'Cappuccino',       'Coffee with foam',          4.5, 150, 'https://images.unsplash.com/photo-1509042239860-f550ce710b93'),
(2, 'Latte',            'Smooth milk coffee',        6.0, 100, 'https://images.unsplash.com/photo-1523942839745-7848d0f5c9d1'),
(3, 'Luxury Watch',     'High-end watch',          500.0,  20, 'https://images.unsplash.com/photo-1523275335684-37898b6baf30'),
(3, 'Designer Handbag', 'Premium leather bag',    1200.0,  15, 'https://images.unsplash.com/photo-1584917865442-de89df76afd3'),
(3, 'RayBan Sunglasses','Stylish sunglasses',      300.0,  10, 'https://images.unsplash.com/photo-1511499767150-a48a237f0083'),
(4, 'Neck Pillow',      'Travel pillow',            25.0, 120, 'https://images.unsplash.com/photo-1540497077202-7c8a3999166f'),
(4, 'Travel Adapter',   'Universal plug adapter',   10.0, 205, 'https://images.unsplash.com/photo-1572635196237-14b3f281503f'),
(4, 'Power Bank',       'Portable charger',         15.0,  90, 'https://images.unsplash.com/photo-1609592424060-bd0c5b305c91');

INSERT INTO Client (name, date_of_birth) VALUES
('Crina',  '2003-03-04'),
('Andrei', '1995-07-15'),
('Maria',  '2001-11-22');

INSERT INTO Cart (client_id, status) VALUES
(1, 'active'),
(2, 'completed'),
(3, 'active');

INSERT INTO CartItem (cart_id, item_id, quantity) VALUES
(1, 1, 2),   -- Crina: 2x Chicken Sandwich
(1, 4, 1),   -- Crina: 1x Espresso
(2, 7, 1),   -- Andrei: 1x Luxury Watch
(2, 10, 2),  -- Andrei: 2x Neck Pillow
(3, 5, 3),   -- Maria: 3x Cappuccino
(3, 12, 1);  -- Maria: 1x Power Bank

INSERT INTO Reservation (cart_id, reservation_date, time_slot, active) VALUES
(1, '2026-04-20', '10:00:00', 1),
(1, '2026-04-21', '14:30:00', 1),
(2, '2026-04-18', '09:15:00', 0),
(3, '2026-04-22', '16:00:00', 1);

INSERT INTO Ticket (category, subcategory) VALUES
('Duty Free Shops', 'Global Duty Free'),
('Duty Free Shops', 'Global Duty Free'),
('Duty Free Shops', 'Sky Bites'),
('Duty Free Shops', 'Runway Cafe'),
('Duty Free Shops', 'Runway Cafe'),
('Duty Free Shops', 'Elite Boutique'),
('Duty Free Shops', 'FlySmart Store'),
('Duty Free Shops', 'FlySmart Store');
GO