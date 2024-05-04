DROP TABLE IF EXISTS auth_person;

CREATE TABLE auth_person (
  login TEXT,
  password TEXT,
  role TEXT
);

INSERT INTO auth_person VALUES
('DenisSt@mail.ru', 'Password_admin1234', 'admin'),
('AlinaSt@mail.ru', 'Password_user1', 'user'),
('IgorSt@mail.ru', 'Password_user2', 'user'),
('StepanSt@mail.ru', 'Password_user3', 'user');