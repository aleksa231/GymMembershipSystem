-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema gymmembershipsystem
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema gymmembershipsystem
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `gymmembershipsystem` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `gymmembershipsystem` ;

-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`roles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`roles` (
  `RoleID` INT NOT NULL AUTO_INCREMENT,
  `RoleName` ENUM('Administrator', 'Employee') NULL DEFAULT NULL,
  PRIMARY KEY (`RoleID`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`users` (
  `UserID` INT NOT NULL AUTO_INCREMENT,
  `Username` VARCHAR(50) NULL DEFAULT NULL,
  `PasswordHash` VARCHAR(255) NULL DEFAULT NULL,
  `RoleID` INT NULL DEFAULT NULL,
  PRIMARY KEY (`UserID`),
  UNIQUE INDEX `Username` (`Username` ASC) VISIBLE,
  UNIQUE INDEX `UC_Username` (`Username` ASC) VISIBLE,
  INDEX `RoleID` (`RoleID` ASC) VISIBLE,
  CONSTRAINT `users_ibfk_1`
    FOREIGN KEY (`RoleID`)
    REFERENCES `gymmembershipsystem`.`roles` (`RoleID`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`activitylog`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`activitylog` (
  `LogID` INT NOT NULL AUTO_INCREMENT,
  `UserID` INT NULL DEFAULT NULL,
  `Action` VARCHAR(100) NULL DEFAULT NULL,
  `ActionDate` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`LogID`),
  INDEX `UserID` (`UserID` ASC) VISIBLE,
  CONSTRAINT `activitylog_ibfk_1`
    FOREIGN KEY (`UserID`)
    REFERENCES `gymmembershipsystem`.`users` (`UserID`))
ENGINE = InnoDB
AUTO_INCREMENT = 4
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`configurations`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`configurations` (
  `ConfigID` INT NOT NULL AUTO_INCREMENT,
  `UserID` INT NULL DEFAULT NULL,
  `PreferredTheme` VARCHAR(50) NULL DEFAULT NULL,
  PRIMARY KEY (`ConfigID`),
  INDEX `UserID` (`UserID` ASC) VISIBLE,
  CONSTRAINT `configurations_ibfk_1`
    FOREIGN KEY (`UserID`)
    REFERENCES `gymmembershipsystem`.`users` (`UserID`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`members`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`members` (
  `MemberID` INT NOT NULL AUTO_INCREMENT,
  `FirstName` VARCHAR(50) NULL DEFAULT NULL,
  `LastName` VARCHAR(50) NULL DEFAULT NULL,
  `PhoneNumber` VARCHAR(15) NULL DEFAULT NULL,
  `Email` VARCHAR(100) NULL DEFAULT NULL,
  `MembershipCardNumber` VARCHAR(20) NULL DEFAULT NULL,
  `JoinDate` DATE NULL DEFAULT NULL,
  `MembershipStatus` ENUM('Active', 'Expired') NULL DEFAULT 'Active',
  PRIMARY KEY (`MemberID`),
  UNIQUE INDEX `UC_Email` (`Email` ASC) VISIBLE,
  UNIQUE INDEX `MembershipCardNumber` (`MembershipCardNumber` ASC) VISIBLE)
ENGINE = InnoDB
AUTO_INCREMENT = 12
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`membernotes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`membernotes` (
  `NoteID` INT NOT NULL AUTO_INCREMENT,
  `MemberID` INT NULL DEFAULT NULL,
  `Note` TEXT NULL DEFAULT NULL,
  `CreatedDate` TIMESTAMP NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`NoteID`),
  INDEX `MemberID` (`MemberID` ASC) VISIBLE,
  CONSTRAINT `membernotes_ibfk_1`
    FOREIGN KEY (`MemberID`)
    REFERENCES `gymmembershipsystem`.`members` (`MemberID`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`membershippackages`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`membershippackages` (
  `PackageID` INT NOT NULL AUTO_INCREMENT,
  `PackageName` VARCHAR(50) NULL DEFAULT NULL,
  `DurationMonths` INT NULL DEFAULT NULL,
  `Price` DECIMAL(10,2) NULL DEFAULT NULL,
  PRIMARY KEY (`PackageID`))
ENGINE = InnoDB
AUTO_INCREMENT = 6
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;


-- -----------------------------------------------------
-- Table `gymmembershipsystem`.`membershippayments`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `gymmembershipsystem`.`membershippayments` (
  `PaymentID` INT NOT NULL AUTO_INCREMENT,
  `MemberID` INT NULL DEFAULT NULL,
  `PackageID` INT NULL DEFAULT NULL,
  `PaymentDate` DATE NULL DEFAULT NULL,
  `AmountPaid` DECIMAL(10,2) NULL DEFAULT NULL,
  PRIMARY KEY (`PaymentID`),
  INDEX `MemberID` (`MemberID` ASC) VISIBLE,
  INDEX `PackageID` (`PackageID` ASC) VISIBLE,
  CONSTRAINT `membershippayments_ibfk_1`
    FOREIGN KEY (`MemberID`)
    REFERENCES `gymmembershipsystem`.`members` (`MemberID`),
  CONSTRAINT `membershippayments_ibfk_2`
    FOREIGN KEY (`PackageID`)
    REFERENCES `gymmembershipsystem`.`membershippackages` (`PackageID`))
ENGINE = InnoDB
AUTO_INCREMENT = 4
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

USE `gymmembershipsystem` ;

-- Insert roles
INSERT INTO roles (RoleName) VALUES 
('Administrator'), 
('Employee');

-- Insert users
INSERT INTO users (Username, PasswordHash, RoleID) VALUES
('marko', 'd74ff0ee8da3b9806b18c877dbf29bbde50b5bd8e4dad7a3a725000feb82e8f1', 2), -- Employee
('admin', '406892809c1d9de3b80b9ba86f7b332c6cd8f8d131d69bb1c3f0ab5c9b668b5f', 1); -- Administrator

-- Insert membership packages
INSERT INTO membershippackages (PackageName, DurationMonths, Price) VALUES
('Monthly', 1, 50.00),
('Quarterly', 3, 135.00),
('Yearly', 12, 480.00);

-- Insert members
INSERT INTO members (FirstName, LastName, PhoneNumber, Email, MembershipCardNumber, JoinDate, MembershipStatus) VALUES
('John', 'Doe', '1234567890', 'john.doe@example.com', 'MC12345', '2024-01-01', 'Active'),
('Jane', 'Smith', '9876543210', 'jane.smith@example.com', 'MC12346', '2024-02-15', 'Active'),
('Robert', 'Brown', '5551234567', 'robert.brown@example.com', 'MC12347', '2024-03-10', 'Active'),
('Emily', 'Davis', '5559876543', 'emily.davis@example.com', 'MC12348', '2024-03-20', 'Expired');

-- Insert membership payments
INSERT INTO membershippayments (MemberID, PackageID, PaymentDate, AmountPaid) VALUES
(1, 1, '2024-01-01', 50.00), -- John - Monthly
(2, 2, '2024-02-15', 135.00), -- Jane - Quarterly
(3, 3, '2024-03-10', 480.00), -- Robert - Yearly
(1, 2, '2024-04-01', 135.00), -- John - Quarterly
(4, 1, '2024-03-20', 50.00); -- Emily - Monthly (Expired)

-- Event for updating membership status

DELIMITER $$

CREATE DEFINER=`root`@`localhost` EVENT `UpdateMembershipStatus`
ON SCHEDULE EVERY 1 DAY
STARTS '2024-12-12 14:31:55'
ON COMPLETION NOT PRESERVE
ENABLE
DO
BEGIN
    -- Update MembershipStatus to 'Expired' for expired memberships
    UPDATE members m
    JOIN membershippayments mp ON m.MemberID = mp.MemberID
    JOIN membershippackages p ON mp.PackageID = p.PackageID
    SET m.MembershipStatus = 'Expired'
    WHERE DATE_ADD(mp.PaymentDate, INTERVAL p.DurationMonths MONTH) < CURDATE();

    -- Delete expired payments
    DELETE FROM membershippayments
    WHERE PaymentID IN (
        SELECT PaymentID
        FROM (
            SELECT mp.PaymentID
            FROM membershippayments mp
            JOIN membershippackages p ON mp.PackageID = p.PackageID
            WHERE DATE_ADD(mp.PaymentDate, INTERVAL p.DurationMonths MONTH) < CURDATE()
        ) AS temp
    );
END$$

DELIMITER ;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
