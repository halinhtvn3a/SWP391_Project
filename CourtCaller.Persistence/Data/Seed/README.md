# Seed Data Files

This directory contains JSON seed data files for the CourtCaller application database.

## Files Overview

### 1. Branches.json
- **Purpose**: Seed data for Branch entities
- **Records**: 5 branches across different locations in Singapore
- **Key Fields**: BranchId, BranchAddress, BranchName, BranchPhone, OpenTime, CloseTime, Status
- **Locations**: Orchard, Marina Bay, Sentosa, Clarke Quay, Bugis

### 2. Courts.json
- **Purpose**: Seed data for Court entities
- **Records**: 14 courts distributed across 5 branches
- **Key Fields**: CourtId, BranchId, CourtName, Status
- **Note**: Each branch has 2-4 courts

### 3. TimeSlots.json
- **Purpose**: Seed data for TimeSlot entities
- **Records**: 18 time slots for different courts
- **Key Fields**: SlotId, CourtId, SlotDate, Price, SlotStartTime, SlotEndTime, Status
- **Note**: All slots are set for 2024-01-15 with different pricing per branch

### 4. Prices.json
- **Purpose**: Seed data for Price entities
- **Records**: 10 price configurations (weekday/weekend for each branch)
- **Key Fields**: PriceId, BranchId, Type, IsWeekend, SlotPrice
- **Note**: Different pricing for weekday vs weekend

### 5. News.json
- **Purpose**: Seed data for News entities
- **Records**: 5 news articles
- **Key Fields**: NewId, Title, Content, PublicationDate, Status, IsHomepageSlideshow
- **Note**: Some articles are marked for homepage slideshow

### 6. UserDetails.json
- **Purpose**: Seed data for UserDetail entities
- **Records**: 5 user profiles with Singapore names
- **Key Fields**: UserId, Balance, Point, FullName, Address, YearOfBirth, IsVip
- **Note**: UserId references IdentityUser emails

### 7. Bookings.json
- **Purpose**: Seed data for Booking entities
- **Records**: 5 sample bookings
- **Key Fields**: BookingId, Id (UserId), BranchId, BookingDate, BookingType, Status, TotalPrice
- **Note**: Mix of Fixed and Flexible booking types

### 8. Payments.json
- **Purpose**: Seed data for Payment entities
- **Records**: 5 payments corresponding to bookings
- **Key Fields**: PaymentId, BookingId, PaymentAmount, PaymentDate, PaymentStatus
- **Note**: Different payment statuses (Success, Pending, Cancelled)

### 9. Reviews.json
- **Purpose**: Seed data for Review entities
- **Records**: 5 reviews from users
- **Key Fields**: ReviewId, Id (UserId), BranchId, ReviewText, Rating, ReviewDate
- **Note**: Ratings range from 3-5 stars

### 10. RegistrationRequests.json
- **Purpose**: Seed data for RegistrationRequest entities
- **Records**: 5 pending registration requests
- **Key Fields**: Id, Email, Password, FullName, Token, TokenExpiration
- **Note**: Used for user registration verification

## Data Relationships

- **Branches** → **Courts** (1:Many)
- **Branches** → **Prices** (1:Many)
- **Branches** → **Reviews** (1:Many)
- **Courts** → **TimeSlots** (1:Many)
- **Bookings** → **Payments** (1:Many)
- **Bookings** → **TimeSlots** (1:Many)

## Usage

These files are automatically loaded by the `CourtCallerDbContextSeed` class during application startup. The seeding process follows this order:

1. Branches
2. Courts
3. TimeSlots
4. Bookings
5. Payments
6. Reviews
7. UserDetails
8. Prices
9. News
10. RegistrationRequests

## Important Notes

- All dates are set to January 2024 for consistency
- User IDs reference IdentityUser emails
- Foreign key relationships are maintained
- All required fields are populated
- Status fields use appropriate enum values
- Prices are in SGD (Singapore Dollar)
- Time formats follow ISO 8601 standard
- Addresses follow Singapore postal code format
- Phone numbers use Singapore country code (+65)

## Validation

Before running the application, ensure:
- All JSON files are valid
- Foreign key relationships are correct
- Date formats are consistent
- Required fields are not null
- Data types match the entity models 