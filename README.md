AgriEnergy System ReadMe

---

System Overview

AgriEnergy is a web-based system designed to facilitate interactions between employees and farmers within an agricultural context. It allows users to manage products and users through a user-friendly interface.

System Functionality

1. User Authentication:
   - Users are authenticated using Firebase Authentication.
   - Only authenticated users (employees or farmers) can access the system.

2. User Roles:
   - Two user roles are defined: Employee (UserRole = 0) and Farmer (UserRole = 1).
   - Employees can manage farmers and products.
   - Farmers can add and manage their own products.

3. User Management:
   - Employees can register new farmers, providing necessary details such as name, email, and password.
   - Farmers register themselves by providing required details.

4. Product Management:
   - Farmers can add new products, specifying product name, category, and production date.
   - Employees can view a list of products supplied by a specific farmer.
   - Product lists can be filtered by date range or product category.

5. Database Interaction:
   - Data is stored in a SQL Server database.
   - The database schema includes tables for Users and Products with appropriate relationships.

6. Web Interface:
   - The web interface is designed to be user-friendly and visually appealing.
   - Consistent styling is applied across all pages for a cohesive user experience.

System Components

1. Controllers:
   - AuthController: Handles user authentication and registration.
   - HomeController: Manages home and error pages.
   - ProductController: Handles product-related actions such as creation, editing, and deletion.

2. Models:
   - User: Represents user data including name, email, role, etc.
   - Product: Represents product data including name, category, production date, etc.

3. Database Context:
   - AgriEnergyContext: Entity Framework DbContext for interacting with the database.

4. Database:
   - A SQL Server database named AgriEnergy is created to store user and product data.

Setup Instructions

1. Database Setup:
   - Create a SQL Server database named AgriEnergy.
   - Run the provided SQL script to create the necessary tables.

2. **Firebase Configuration**:
   - Obtain Firebase configuration credentials and set them as environment variables (AgriEnergyFirebase).

3. ASP.NET Core Setup:
   - Ensure the ASP.NET Core environment is configured properly.
   - Set up appropriate dependencies and packages using NuGet Package Manager.

4. Running the Application:
   - Build and run the application in your preferred development environment.

5. Accessing the Application:
   - Access the application through a web browser using the provided URL.
   - Users must register and log in to access system features.

Contributors

- Ethan Swanepoel

License

This project is licensed under the [License Name] License - see the [LICENSE.md](LICENSE.md) file for details.

---

