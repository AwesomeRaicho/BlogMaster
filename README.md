# Project Name: BlogMaster

## Purpose

The main goal of the application is to have an easily available ASP.NET server template for creating a blog website of any subject.

## Target Audience

Developers who are already familiar with ASP.NET and would like a built-in administrator UI for managing the website content.

## Key Features/Functionality

- Views designed for administrators of the website
- Creatable users with roles for additional writers and editors with specific privileges
- Easily add, remove, edit, publish, and unpublish blogs with images
- Subscription system for users/customers, with different tier access layers, as well as a donation system

## Technology Stack

- **Front-end**: Server-side rendering with Razor views, some JavaScript for required functionality.
  - **Note**: Decided to go with Razor views (server-side rendering) instead of a front-end framework like React because it’s a blog website and it may be better for search engine crawlers.
- **Back-end**: ASP.NET MVC design using a clean architecture (UI, Core, and Infrastructure class libraries), the Entity Framework ORM with Identity system for the database layer which will be implemented for SQL Server and for user authorization and authentication.

## Scope

The project will only include the boilerplate code so that a developer can adapt it with a different database if needed. The Core class library will be designed to return lists for the controllers to provide data to the views (No API implementation is planned as of now).

## Model Design

Payment, Subscription and SubscriptionHistory tables are WIP.

![Entity Graph](image.png)
