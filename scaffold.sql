-- Dumping database structure for solid price
CREATE DATABASE IF NOT EXISTS `solid price` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `solid price`;

-- Dumping structure for table solid price.cutitems
CREATE TABLE IF NOT EXISTS `cutitems` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `StockItemID` int NOT NULL,
  `Qty` int NOT NULL,
  `Length` float NOT NULL,
  `Angle1` float NOT NULL,
  `Angle2` float NOT NULL,
  `AngleDirection` text,
  `AngleRotation` text,
  `StickNumber` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `IX_CutItems_StockItemID` (`StockItemID`),
  CONSTRAINT `FK_CutItems_StockItems_StockItemID` FOREIGN KEY (`StockItemID`) REFERENCES `stockitems` (`ID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=45 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.cutitems: ~4 rows (approximately)
/*!40000 ALTER TABLE `cutitems` DISABLE KEYS */;
INSERT INTO `cutitems` (`ID`, `StockItemID`, `Qty`, `Length`, `Angle1`, `Angle2`, `AngleDirection`, `AngleRotation`, `StickNumber`) VALUES
	(41, 2, 1, 4.13, 16.68, 0, '-', '-', 1),
	(42, 2, 1, 2.787, 0, 16.68, '-', '-', 1),
	(43, 1, 1, 4.28, 16.68, 0, '-', '-', 1),
	(44, 1, 1, 2.937, 0, 16.68, '-', '-', 1);
/*!40000 ALTER TABLE `cutitems` ENABLE KEYS */;

-- Dumping structure for table solid price.orderitems
CREATE TABLE IF NOT EXISTS `orderitems` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `StockItemID` int NOT NULL,
  `Qty` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `IX_OrderItems_StockItemID` (`StockItemID`),
  CONSTRAINT `FK_OrderItems_StockItems_StockItemID` FOREIGN KEY (`StockItemID`) REFERENCES `stockitems` (`ID`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.orderitems: ~2 rows (approximately)
/*!40000 ALTER TABLE `orderitems` DISABLE KEYS */;
INSERT INTO `orderitems` (`ID`, `StockItemID`, `Qty`) VALUES
	(21, 1, 1),
	(22, 2, 1);
/*!40000 ALTER TABLE `orderitems` ENABLE KEYS */;

-- Dumping structure for table solid price.sheetcutitems
CREATE TABLE IF NOT EXISTS `sheetcutitems` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `SheetStockItemID` int NOT NULL,
  `Qty` int NOT NULL,
  `Length` float NOT NULL,
  `Width` float NOT NULL,
  `GrainDirection` int NOT NULL,
  `SheetNumber` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `IX_SheetCutItems_SheetStockItemID` (`SheetStockItemID`),
  CONSTRAINT `FK_SheetCutItems_SheetStockItems_SheetStockItemID` FOREIGN KEY (`SheetStockItemID`) REFERENCES `sheetstockitems` (`ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.sheetcutitems: ~0 rows (approximately)
/*!40000 ALTER TABLE `sheetcutitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `sheetcutitems` ENABLE KEYS */;

-- Dumping structure for table solid price.sheetorderitems
CREATE TABLE IF NOT EXISTS `sheetorderitems` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `SheetStockItemID` int NOT NULL,
  `Qty` int NOT NULL,
  PRIMARY KEY (`ID`),
  KEY `IX_SheetOrderItems_SheetStockItemID` (`SheetStockItemID`),
  CONSTRAINT `FK_SheetOrderItems_SheetStockItems_SheetStockItemID` FOREIGN KEY (`SheetStockItemID`) REFERENCES `sheetstockitems` (`ID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.sheetorderitems: ~0 rows (approximately)
/*!40000 ALTER TABLE `sheetorderitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `sheetorderitems` ENABLE KEYS */;

-- Dumping structure for table solid price.sheetstockitems
CREATE TABLE IF NOT EXISTS `sheetstockitems` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `MatType` int NOT NULL,
  `StockLengthInInches` float NOT NULL,
  `StockWidthInInches` float NOT NULL,
  `Thickness` float NOT NULL,
  `Finish` text,
  `InternalDescription` text,
  `ExternalDescription` text,
  `VendorID` int DEFAULT NULL,
  `CostPerSqFoot` decimal(18,2) NOT NULL,
  `VendorItemNumber` text,
  PRIMARY KEY (`ID`),
  KEY `IX_SheetStockItems_VendorID` (`VendorID`),
  CONSTRAINT `FK_SheetStockItems_Vendors_VendorID` FOREIGN KEY (`VendorID`) REFERENCES `vendors` (`ID`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.sheetstockitems: ~0 rows (approximately)
/*!40000 ALTER TABLE `sheetstockitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `sheetstockitems` ENABLE KEYS */;

-- Dumping structure for table solid price.stockitems
CREATE TABLE IF NOT EXISTS `stockitems` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `MatType` int NOT NULL,
  `ProfType` int NOT NULL,
  `StockLength` float NOT NULL,
  `InternalDescription` text,
  `ExternalDescription` text,
  `VendorID` int DEFAULT NULL,
  `CostPerFoot` decimal(18,2) NOT NULL,
  `VendorItemNumber` text,
  PRIMARY KEY (`ID`),
  KEY `IX_StockItems_VendorID` (`VendorID`),
  CONSTRAINT `FK_StockItems_Vendors_VendorID` FOREIGN KEY (`VendorID`) REFERENCES `vendors` (`ID`) ON DELETE RESTRICT
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.stockitems: ~2 rows (approximately)
/*!40000 ALTER TABLE `stockitems` DISABLE KEYS */;
INSERT INTO `stockitems` (`ID`, `MatType`, `ProfType`, `StockLength`, `InternalDescription`, `ExternalDescription`, `VendorID`, `CostPerFoot`, `VendorItemNumber`) VALUES
	(1, 2, 10, 24, '2in x 2in x .125in Alum. Square Tube', '2in x 2in x .125in Alum. Square Tube', 1, 0.00, ''),
	(2, 2, 10, 24, '1in x 1in x .125in Alum. Square Tube', '1in x 1in x .125in Alum. Square Tube', 1, 0.00, '');
/*!40000 ALTER TABLE `stockitems` ENABLE KEYS */;

-- Dumping structure for table solid price.vendors
CREATE TABLE IF NOT EXISTS `vendors` (
  `ID` int NOT NULL AUTO_INCREMENT,
  `VendorName` text,
  `PhoneNumber` text,
  `ContactName` text,
  `ContactEmail` text,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.vendors: ~0 rows (approximately)
/*!40000 ALTER TABLE `vendors` DISABLE KEYS */;
INSERT INTO `vendors` (`ID`, `VendorName`, `PhoneNumber`, `ContactName`, `ContactEmail`) VALUES
	(1, 'N/A', 'N/A', 'N/A', 'N/A');
/*!40000 ALTER TABLE `vendors` ENABLE KEYS */;

-- Dumping structure for table solid price.__efmigrationshistory
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Dumping data for table solid price.__efmigrationshistory: ~0 rows (approximately)
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
	('20210825161654_initial', '5.0.9'),
	('20210910192710_addsheetfunctionality', '5.0.9'),
	('20210916021611_addsheetfunctionality', '5.0.10');
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
