DELETE FROM [TABLE]
DBCC CHECKIDENT ('Pub.dbo.TABLE', RESEED, 0)
DELETE FROM [GUEST]
DBCC CHECKIDENT ('Pub.dbo.GUEST', RESEED, 0)
DELETE FROM [MENU_ITEM]
DBCC CHECKIDENT ('Pub.dbo.MENU_ITEM', RESEED, 0)
DELETE FROM [WAITER]
DBCC CHECKIDENT ('Pub.dbo.WAITER', RESEED, 0)
DELETE FROM [ORDER]
DBCC CHECKIDENT ('Pub.dbo.ORDER', RESEED, 0)

INSERT INTO "TABLE"("Number", "Seats", "Status")
VALUES
(101, 5, 'Available'), --ID=1
(102, 2, 'Full'), --ID=2
(103, 3, 'Occupied'), --ID=3
(104, 7, 'Occupied'), --ID=4
(105, 10, 'Available'), --ID=5
(106, 3, 'Available'), --ID=6
(107, 3, 'Full'), --ID=7
(108, 5, 'Occupied'); --ID=8

INSERT INTO "GUEST"("Name", "Money", "HasAllergies", "HasDiscount", "TableID")
VALUES
('Dusan', 1000, 0, 0, 2), --ID=1
('Stefan', 800, 0, 1, 2), --ID=2
('Ana', 1370, 0, 1, 3), --ID=3
('Boris', 960, 1, 0, 3), --ID=4
('Ena', 2800, 1, 0, 4), --ID=5
('Marko', 500, 1, 1, 4), --ID=6
('Nikola', 1264, 1, 0, 4), --ID=7
('Jovana', 1110, 0, 0, 7), --ID=8
('Ivana', 735, 1, 1, 7), --ID=9
('Ilija', 1234, 1, 0, 7), --ID=10
('Pavle', 999, 0, 1, 8); --ID=11

INSERT INTO "MENU_ITEM"("Name", "Price", "Category", "HasAllergens")
VALUES
('Burger', 350, 'International', 0), --ID=1
('Dumplings', 100, 'Chinese', 0), --ID=2
('Snails', 460, 'French', 1), --ID=3
('Chicken Curry', 300, 'Indian', 1), --ID=4
('Pasta Carbonara', 450, 'Italian', 1), --ID=5
('Fish and Chips', 290, 'International', 0), --ID=6
('Pizza Margherita', 500, 'Italian', 0), --ID=7
('Sushi', 800, 'Japanese', 1), --ID=8
('Ramen', 450, 'Japanese', 0), --ID=9
('Tacos', 390, 'Mexican', 0), --ID=10
('Cheese Cake', 300, 'International', 0), --ID=11
('Ice Cream', 150, 'International', 0); --ID=12

INSERT INTO "WAITER"("Name", "Tips")
VALUES
('David', 0), --ID=1
('Dimitrije', 100), --ID=2
('Sara', 600), --ID=3
('Kristina', 440), --ID=4
('Andjelija', 120), --ID=5
('Mihajlo', 0); --ID=6

INSERT INTO "ORDER"("OrderTime", "Status", "Quantity", "GuestID", "MenuItemID", "WaiterID")
VALUES
('2024-03-19T12:30:00', 'Preparing', 3, 1, 10, null), --ID=1
('2024-03-19T12:33:20', 'Pending', 1, 2, 8, null), --ID=2
('2024-03-19T12:31:18', 'Preparing', 2, 2, 6, null), --ID=3
('2024-03-19T12:37:02', 'Pending', 5, 3, 4, null), --ID=4
('2024-03-19T12:38:48', 'Completed', 1, 4, 2, null), --ID=5
('2024-03-19T12:40:33', 'Completed', 2, 4, 11, null), --ID=6
('2024-03-19T12:47:30', 'Completed', 1, 4, 1, null), --ID=7
('2024-03-19T12:49:42', 'Delivered', 1, 5, 10, 3), --ID=8
('2024-03-19T12:50:12', 'Pending', 3, 1, 5, null), --ID=9
('2024-03-19T12:58:55', 'Delivered', 3, 7, 7, 1); --ID=10