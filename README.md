# BlogMaster

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

### Application URLs

The application will run on either of the following URLs:

- `https://localhost:7218`
- `http://localhost:5048`

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

# Stripe Payments Integration

This application integrates **Card Payments** using embedded sessions from the Stripe API.

### Payment Endpoints

In the `PaymentController`, you will find endpoints to handle both **donation** and **subscription** payments:

- **Subscription Checkout Page**: `"/checkout-subscription"`
- **Donation Checkout Page**: `"/checkout-donation"`

These pages display embedded payment forms fetched from the following endpoints:

- **Embedded Form for Subscription Checkout**: `"/checkout-session-subscription"`
- **Embedded Form for Donation Checkout**: `"/create-checkout-session-donation"`

### Payment Method Management

A separate form for adding new payment methods (credit cards) is located in the `SubscriptionController`:

- **Payment Methods Page**: `"/payment-methods"`
- **Endpoint for Adding New Payment Methods**: `"/create-payment-method"`

The `/create-payment-method` form is embedded in a modal that opens when the "Add Payment Method" button is clicked.

![alt text](image-1.png)
![alt text](image-2.png)

### Payment Handling and Card Management

The `"/payment-return"` endpoint handles responses from Stripe when new cards are added. Initial subscription and donation payments automatically save the user's card as a payment method, which can later be managed and removed on the Payment Methods page.

![alt text](image-3.png)

## Stripe Business Logic

All Stripe-related functionality, including monthly subscription payments and paywalled blog access, resides in the `StripeService` class (interfaced by `IStripeService`). This service class is responsible for communication with the Stripe API.

![alt text](image-4.png)

**Note**: `StripeService` currently contains two hardcoded properties:

- **Price**: Defines the cost of the subscription.
- **Domain**: Specifies the domain for the return URL used in adding new payment methods.

These values can be updated in `StripeService` for different pricing and domain configurations.

## Use Diagram

![alt text](image-5.png)
