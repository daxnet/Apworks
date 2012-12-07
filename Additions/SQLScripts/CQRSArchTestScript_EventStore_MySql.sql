delimiter $$

CREATE DATABASE `apworkscqrsarcheventstoretestdb` /*!40100 DEFAULT CHARACTER SET latin1 */$$

delimiter $$
USE `apworkscqrsarcheventstoretestdb`$$

delimiter $$

CREATE TABLE `domainevents` (
  `Id` varchar(40) NOT NULL,
  `SourceID` varchar(40) NOT NULL,
  `AssemblyQualifiedSourceType` text NOT NULL,
  `Timestamp` datetime NOT NULL,
  `Version` bigint(20) NOT NULL,
  `Branch` bigint(20) NOT NULL,
  `AssemblyQualifiedEventType` text NOT NULL,
  `Data` longblob NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1$$

delimiter $$

CREATE TABLE `snapshots` (
  `Id` varchar(40) NOT NULL,
  `Timestamp` datetime NOT NULL,
  `SnapshotData` longblob NOT NULL,
  `AggregateRootID` varchar(40) NOT NULL,
  `AggregateRootType` text NOT NULL,
  `SnapshotType` text NOT NULL,
  `Version` bigint(20) NOT NULL,
  `Branch` bigint(20) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1$$

