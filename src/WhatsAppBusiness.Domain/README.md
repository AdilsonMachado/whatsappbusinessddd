# WhatsApp Business Domain Layer

This is the core Domain layer of the WhatsApp Business DDD application. It contains the business logic, entities, value objects, and domain services following Domain-Driven Design principles.

## Structure

### Common (`/Common`)
Base classes for the domain model:
- **Entity.cs** - Base class for all entities with GUID identifier and equality comparison
- **ValueObject.cs** - Base class for all value objects with structural equality
- **DomainEvent.cs** - Base class for domain events

### Entities (`/Entities`)
Rich domain entities with encapsulated business logic:
- **User** - Represents a user in the system with authentication state
- **Operator** - Represents a human operator who can handle conversations
- **Conversation** - Represents a conversation between a user and the system
- **Message** - Represents a message in a conversation
- **WhatsAppConfiguration** - Represents WhatsApp API configuration

### Value Objects (`/ValueObjects`)
Immutable value objects:
- **PhoneNumber** - Phone number with international format validation
- **MessageContent** - Message content with validation (max 4096 characters)
- **ConversationStatus** - Enum for conversation states (WaitingAuthentication, WithAI, WithOperator, Closed)
- **MessageDirection** - Enum for message direction (Incoming, Outgoing)
- **MessageType** - Enum for message types (Text, Image, Audio, Video, Document, Location, Contact, Sticker)

### Repository Interfaces (`/Interfaces/Repositories`)
Contracts for data access:
- **IUserRepository** - User data access operations
- **IOperatorRepository** - Operator data access operations
- **IConversationRepository** - Conversation data access operations
- **IMessageRepository** - Message data access operations
- **IConfigurationRepository** - WhatsApp configuration data access operations

### Domain Services (`/Services`)
Business logic that doesn't belong to a single entity:
- **AuthenticationService** - User authentication and registration logic
- **ConversationFlowService** - Conversation flow and operator assignment logic

### Domain Events (`/Events`)
Events raised by domain entities:
- **MessageReceivedEvent** - Raised when a message is received from a user
- **UserAuthenticatedEvent** - Raised when a user is successfully authenticated
- **ConversationAssignedToOperatorEvent** - Raised when a conversation is assigned to an operator

## Key Features

### DDD Principles
- **Rich Domain Model** - Entities contain business logic and enforce invariants
- **Encapsulation** - Private setters with factory methods and behavioral methods
- **Value Objects** - Immutable objects with validation
- **Repository Pattern** - Abstract data access behind interfaces
- **Domain Events** - Communicate state changes across the domain
- **Domain Services** - Coordinate complex business processes

### C# Features
- File-scoped namespaces
- Nullable reference types enabled
- Record-like equality for value objects
- Factory methods for entity creation
- Guard clauses for validation

## Usage Examples

### Creating a User
```csharp
var phoneNumber = PhoneNumber.Create("+1234567890");
var user = User.Create("John Doe", phoneNumber);
user.Authenticate();
```

### Creating a Conversation
```csharp
var phoneNumber = PhoneNumber.Create("+1234567890");
var conversation = Conversation.Create(userId, phoneNumber);
conversation.MarkAsAuthenticated();
conversation.AssignToOperator(operatorId);
```

### Creating a Message
```csharp
var content = MessageContent.Create("Hello, world!");
var message = Message.Create(
    conversationId,
    content,
    MessageType.Text,
    MessageDirection.Incoming,
    isFromAI: false);
message.MarkAsDelivered();
message.MarkAsRead();
```

### Using Domain Services
```csharp
// Authentication
var (user, conversation) = await authService.AuthenticateUserAsync(
    phoneNumber, 
    userName, 
    cancellationToken);

// Conversation Flow
var operator = await conversationFlowService.AssignConversationToOperatorAsync(
    conversationId, 
    cancellationToken);
```

## Design Decisions

### Entity Identity
All entities use GUID for identity to ensure global uniqueness and avoid database dependencies.

### Validation Strategy
- Value objects validate on creation (fail-fast)
- Entities validate in factory methods and behavioral methods
- Exceptions are thrown for invalid state transitions

### Aggregate Boundaries
- User is a separate aggregate
- Conversation is an aggregate root that coordinates with Messages
- Operator is a separate aggregate
- Messages are children of Conversations

### Immutability
- Value objects are immutable
- Entities use private setters and expose behavioral methods
- Domain events are immutable

## Dependencies
This layer has **no external dependencies** and only uses .NET 9.0 framework libraries, maintaining the purity of the domain model.
