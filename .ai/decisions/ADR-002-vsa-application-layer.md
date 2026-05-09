# ADR-002: Vertical Slice Architecture in Application Layer

## Status: Accepted

## Context
Application layers are traditionally organized by type (Commands/, Queries/, DTOs/).
This creates scattered navigation when working on a feature.

## Decision
Organize Application by feature slice: Features/Products/CreateProduct/ contains
the command, handler, and any slice-specific mappings.

## Rationale
- A developer working on CreateProduct touches one folder
- Handlers can have slice-specific response shapes without polluting shared DTOs
- Demonstrates awareness of modern .NET application patterns
- Shared concerns (behaviours, interfaces, exceptions) remain in Common/

## Consequences
- Some DTOs are duplicated across slices (e.g. ProductSummaryDto vs ProductDto)
  - this is intentional; slices own their response contracts
- ProductSearchEngine lives inside Features/Products/SearchProducts/ - it belongs to the slice that uses it
- Shared application contracts belong to Common/, not a slice
