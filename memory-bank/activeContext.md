## Active Context

- Updated `IntellishelfApiClient.cs` to accept email for login.
- Implemented pagination for book list with infinite scrolling.
- Added sorting capabilities for books by Title, Author, Published date, and Added date.
- Refactored Books page to remove complex ViewModel dependency for POC simplicity.

## Current Focus Areas
1. Books page now uses direct code-behind implementation without ViewModel complexity.

## Recent Changes
- Created initial memory bank structure
- Updated API integration documentation
- Refactored book list virtualization
- Implemented SQLite local cache
- Refactored `IntellishelfApiClient` to use a `BaseApiClient` for common logic (token handling, request sending).
- Updated `AuthToken` model and login process to handle Access Token, Refresh Token, and Expiry Date from API.
- Implemented pagination for book list with infinite scrolling and sorting capabilities.
- Implemented logic to render book cover on the Books page, using the provided API endpoint.
- **MAJOR**: Refactored Books.xaml and Books.xaml.cs to eliminate BooksViewModel dependency - moved all logic directly to code-behind for POC simplicity.

## Key Decisions
- Using SQLite-net for local storage
- Adopting CommunityToolkit.MVVM patterns
- Implementing exponential backoff for API retries
- Centralized API request logic in `BaseApiClient`.
- Storing Access Token, Refresh Token, and Expiry Date in `Preferences`.

## Next Steps
1. Finalize memory bank documentation
2. Implement conflict resolution strategy
3. Optimize image loading performance
4. Add background sync service
5. Implement chat session persistence
6. Implement token refresh logic using the stored Refresh Token and Expiry Date (future task).

## Active Considerations
- Balancing offline capabilities with storage limits
- MAUI Android/iOS feature parity
- Secure storage for authentication tokens (currently using `Preferences`, consider `SecureStorage` for production).
- Token refresh strategy implementation.
