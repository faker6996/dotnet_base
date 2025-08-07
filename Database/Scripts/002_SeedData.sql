-- Insert sample users
INSERT INTO users (username, email, full_name) VALUES
('admin', 'admin@example.com', 'System Administrator'),
('user1', 'user1@example.com', 'John Doe'),
('user2', 'user2@example.com', 'Jane Smith');

-- Insert sample accounts
INSERT INTO accounts (account_name, account_type, balance, is_active) VALUES
('Main Account', 'CHECKING', 5000.00, true),
('Savings Account', 'SAVINGS', 15000.00, true),
('Business Account', 'BUSINESS', 25000.00, true),
('Investment Account', 'INVESTMENT', 50000.00, true);