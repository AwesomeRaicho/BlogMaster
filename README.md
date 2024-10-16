# Project Name: BlogMaster

## Key Features/Functionality

- Views designed for administrators of the blog website
- Creatable users with roles for additional writers and editors with specific privileges
- Easily add, remove, edit, publish, and unpublish blogs with images
- Subscription system for users/customers, with different tier access layers, as well as a donation system

## Technology Stack

- **Front-end**: Server-side rendering with Razor views, JavaScript and Bootstrap for required functionality.
  - **Note**: Decided to go with Razor views (server-side rendering) instead of a SPA front-end framework like React because it’s a blog website and it may be better for search engine crawlers.
- **Back-end**: ASP.NET MVC design using a clean architecture (UI, Core, and Infrastructure class libraries), Entity Framework ORM with Identity Framework system for the database layer for user authorization and authentication.

## Getting Started

### Run the Application

To run the application, use the following command from the `BlogMaster` directory in your CLI:

```bash
dotnet run
```

### Stripe and SMTP Server Configuration

Create a `Secret.json` file and add it to the `BlogMaster` directory with the following json information:

```json
{
  "EmailSettings": {
    "SmtpServer": "[YOUR-SMTP-SERVER].com",
    "SmtpPort": [YOUR-PORT],
    "UseSsl": [true/false],
    "Username": "[YOUR-USERNAME]",
    "Password": "[YOUR-PASSWORD]",
    "SenderEmail": "[YOUR-SENDER-EMAIL]"
  },
  "Stripe": {
    "PublishableKey": "[YOUR-PUBLISHABLE-KEY]",
    "SecretKey": "[YOUR-SECRET-KEY]",
    "SecretEndpoint": "[YOUR-SECRET-ENDPOINT]",
    "TestingEndpoint": "[YOUR-TESTING-ENDPOINT]"
  }
}
```

## Modeling Tables

![Entity Graph](image.png)

### Seeding Admin Users

In `Models/ModelBuilderExtensions.cs`, you will find the data being seeded. Be sure to modify this information to suit your preferences.

**Default Admin User:**

- **Username**: Admin
- **Password**: adminpass
