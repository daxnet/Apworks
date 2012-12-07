Project Description
===================
Apworks is a flexible, scalable, configurable and efficient .NET based application development framework that helps software developers to easily build enterprise applications by applying either Classic Layering or Command-Query Responsibility Segregation (CQRS) architectural patterns. Apworks provides the fundamental libraries and tools for practicing and implementing Domain-Driven Design concepts such as entities, value objects, repositories, factories, specifications, event sourcing, snapshots, domain repositories, message dispatching and synchronization, etc. It also provides the utilities from infrastructure level such as Inversion of Control/Dependency Injection (IoC/DI) components, AOP interception, exception handling & logging, so that architects and developers can focus on the business domain and communicate with domain experts via ubiquitous language without any concern of technical implementations.

Followings are the highlights of current implementation of Apworks framework:

- *Flexible configuration* 每 Apworks provides both config file configuration (app/web.config) and coding configuration. Config file configuration allows system administrators to change the application configuration after the application has been deployed, it is usually used in the production environment. The coding configuration allows developers to build the configuration information for Apworks in the source code so that developers would benefit from the IntelliSense provided by Visual Studio. Furthermore, coding configuration also allows multiple configuration information exists under the same AppDomain, this makes it easier to have your application unit-tested
- *Message bus integration* 每 Apworks provides a flexible framework to support message buses. Message buses are important building blocks for distributed enterprise solutions, applications would dispatch or receive messages (like events) in different patterns. By this means, systems under the same solution can be easily and loosely integrated. Apworks provides both Direct Message Bus, which dispatches messages directly to another process, and MSMQ Message Bus, which utilizes the Microsoft Message Queuing (MSMQ) to perform message dispatching. Also, Apworks allows developers to extend the framework so that 3rd-party messaging solutions can be integrated into their own business
- *Supports different database systems* 每 Developers can choose different database systems according to the project＊s needs. When applying Classic Layering architectural pattern, Apworks provides an adapter for NHibernate ORM so that different databases can be used in the project. For CQRS application architecture, Apworks not only provides Relational-Database Management System (RDBMS) adapters such as SQL Server and MySQL, but also provides the interfaces for extending the framework to use NoSQL solutions
- *Supports different domain repositories* 每 Apworks provides following implementation for domain repositories to meet different project scenarios:
	- Event Sourced Domain Repository 每 The domain repository used for event sourcing. If you need to enable the event sourcing mechanism in your project, that＊s it
	- Regular Domain Repository 每 The domain repository that applies the standard repository pattern. This domain repository will store the entities themselves in the backend database, rather than domain events, so you won＊t get any benefits from the event sourcing
	- Regular Event Publisher Domain Repository 每 This domain repository will store entities rather than domain events in the backed database, however, compared with the Regular Domain Repository, it will emit domain events and publish them to the message bus so that external systems could be easily integrated
	- Snapshot Domain Repository 每 This domain repository will take snapshots for the entities to be saved and stores the snapshots
- *Supports different snapshot providers* 每 When applying CQRS, snapshots are very important when event data are growing larger and larger. Apworks supports two built-in snapshot provides:
	- Event Number Snapshot Provider 每 This snapshot provider counts the number of events generated for a specific aggregate root. If the number exceeds a specific amount (maximum number of events), a snapshot will be taken from the aggregate root and be stored to the snapshot database. This maximum number of events is configurable in the Apworks＊ configuration
	- Suppressed Snapshot Provider 每 Use this snapshot provider along with the Event Sourced Domain Repository to suppress the generating of the snapshots
- *Aspect-oriented programming (AOP)* 每 By utilizing the dynamic proxy of Castle project, Apworks is now able to provide the features of Aspect-oriented programming. From the Apworks＊ configuration, developers are able to specify the methods that need to be intercepted as well as the interceptors to achieve their AOP goal
- *Exception handling* 每 Apworks provides a built-in interceptor for working with exception handling mechanism. Exception handling is also highly configurable from Apworks configuration

Dependencies
============
Apworks has applied the Separated Interface Pattern to reduce the dependencies on other 3rd-party frameworks & tools from its core level. However for some adapter components, they still depends on specific technical implementation and 3rd-party libraries. These dependencies include:

- Object containers 每 Apworks uses Microsoft Unity as IoC/DI container, however developers can use the interfaces defined in Apworks core assembly to customize object containers
- Repositories for Classic Layering architectural pattern 每 Apworks uses NHibernate ORM to implement the repository for Classic Layering architecture, however developers can use the interfaces defined in Apworks core assembly to customize repositories
- SQL Server database storages 每 This depends on the SQL Server client components defined under System.Data.SqlClient namespace of Microsoft .NET Framework 4.0
- MySQL database storages 每 This depends on the MySQL .NET Connector components
- Aspect-oriented programming (AOP) 每 This depends on the dynamic proxy facilities provided by Castle Core assembly

Note that in the standard Apworks distribute package all required assemblies/libraries are included, you don＊t need to download them and replace the ones in the package individually. Apworks will announce the replacement of latest 3rd-party components in the release notes.

Sample Project
==============
The [Tiny Library CQRS](http://tlibcqrs.codeplex.com "Tiny Library CQRS") project fully demonstrates how to apply CQRS architectural pattern in a real-world application by using Apworks framework.

![Tiny Library CQRS](http://download.codeplex.com/Download?ProjectName=apworks&DownloadId=276386)