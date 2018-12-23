-- MySQL Script handcrafted

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema CodyMaze
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `CodyMaze` DEFAULT CHARACTER SET utf8;
USE `CodyMaze`;

-- -----------------------------------------------------
-- Table `CodyMaze`.`Moves`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `CodyMaze`.`Moves` (
  `ID` INT UNSIGNED NOT NULL AUTO_INCREMENT,
  `AlexaSessionID` VARCHAR(256) NOT NULL,
  `AlexaUserID` VARCHAR(512) NOT NULL,
  `Coordinates` CHAR(2) NOT NULL,
  `Direction` CHAR(1) DEFAULT NULL,
  `CreationTime` DATETIME NOT NULL,
  `ReachedOn` DATETIME DEFAULT NULL,
  PRIMARY KEY (`ID`),
  INDEX `Session_idx` (`AlexaSessionID`),
  INDEX `User_idx` (`AlexaUserID`),
  INDEX `Timestamp_idx` (`CreationTime`)
)
ENGINE = InnoDB;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
