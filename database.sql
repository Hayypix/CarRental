-- Скрипт создания базы данных "Автопрокат" (SQLite)
-- Этот же скрипт выполняется программой автоматически при первом запуске.

DROP TABLE IF EXISTS Rentals;
DROP TABLE IF EXISTS Clients;
DROP TABLE IF EXISTS Cars;
DROP TABLE IF EXISTS Users;

-- пользователи системы
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Login TEXT NOT NULL,
    Password TEXT NOT NULL
);

-- автомобили
CREATE TABLE Cars (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Brand TEXT NOT NULL,           -- марка
    Model TEXT NOT NULL,           -- модель
    Year INTEGER,                  -- год выпуска
    Plate TEXT,                    -- гос. номер
    PricePerDay REAL NOT NULL,     -- цена за сутки, руб.
    Status TEXT NOT NULL DEFAULT 'Свободен'   -- Свободен / В аренде
);

-- клиенты
CREATE TABLE Clients (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL,        -- ФИО
    Phone TEXT,                    -- телефон
    Passport TEXT,                 -- паспорт
    Address TEXT                   -- адрес
);

-- договоры проката
CREATE TABLE Rentals (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CarId INTEGER NOT NULL,        -- автомобиль
    ClientId INTEGER NOT NULL,     -- клиент
    StartDate TEXT NOT NULL,       -- дата выдачи (гггг-мм-дд)
    EndDate TEXT,                  -- плановая дата возврата
    ReturnDate TEXT,               -- фактическая дата возврата
    TotalCost REAL,                -- стоимость, руб.
    Status TEXT NOT NULL DEFAULT 'Открыт'   -- Открыт / Закрыт
);

INSERT INTO Users (Login, Password) VALUES ('admin', 'admin');

INSERT INTO Cars (Brand, Model, Year, Plate, PricePerDay, Status) VALUES
('Lada', 'Vesta', 2020, 'А321ТК77', 2500, 'Свободен'),
('Kia', 'Rio', 2019, 'В712НО77', 2800, 'Свободен'),
('Hyundai', 'Solaris', 2021, 'Е405РС77', 3000, 'Свободен'),
('Toyota', 'Camry', 2018, 'К918ММ77', 5500, 'В аренде'),
('Renault', 'Logan', 2017, 'О234КХ77', 2200, 'Свободен'),
('Skoda', 'Octavia', 2019, 'Т600АА77', 3800, 'Свободен');

INSERT INTO Clients (FullName, Phone, Passport, Address) VALUES
('Иванов Сергей Петрович', '+7 (912) 345-67-89', '4509 123456', 'г. Москва, ул. Ленина, 15'),
('Петрова Анна Викторовна', '+7 (922) 654-32-10', '4508 654321', 'г. Москва, ул. Мира, 24'),
('Сидоров Дмитрий Александрович', '+7 (902) 111-22-33', '4507 987654', 'г. Москва, пр. Победы, 5'),
('Козлова Мария Андреевна', '+7 (919) 777-88-99', '4506 456789', 'г. Москва, ул. Садовая, 3');

INSERT INTO Rentals (CarId, ClientId, StartDate, EndDate, ReturnDate, TotalCost, Status) VALUES
(2, 1, '2026-09-01', '2026-09-06', '2026-09-06', 14000, 'Закрыт'),
(4, 3, '2026-09-28', '2026-10-02', NULL, 22000, 'Открыт');
