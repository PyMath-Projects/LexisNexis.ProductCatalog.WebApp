# Product Catalog - Project Overview

## What This Is
A Product Catalog Management System built as a senior C#/Angular developer assessment.
The goal is to demonstrate production-grade architecture in a focused demo context.

## Business Domain
An e-commerce administrator manages a product catalog: creating products, organising them
into hierarchical categories, tracking stock levels, and searching the catalog.

## Key Constraints
- This is an assessment - scope is intentionally bounded
- Authentication is deliberately excluded (BFF scaffold is ready for it)
- No external message broker - domain events are in-process
- EF Core InMemory - spec-compliant, no migrations required

## Non-Goals
- Payment processing
- Order management
- Authentication/authorisation (scaffolded for later)
- Multi-tenancy
- Real-time notifications (WebSocket / SignalR)

## Stack
Backend: .NET 9, ASP.NET Core, EF Core InMemory, MediatR
Frontend: Angular 17, TypeScript, Tailwind CSS, RxJS
BFF: .NET 9, YARP (custom thin BFF, no third-party BFF framework)
Tests: xUnit, FluentAssertions, Moq

## Ports (Development)
- API:  http://localhost:5000
- BFF:  http://localhost:5100
- UI:   http://localhost:4200
