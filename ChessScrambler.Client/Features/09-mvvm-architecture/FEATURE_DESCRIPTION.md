# MVVM Architecture Feature

## Overview
The MVVM (Model-View-ViewModel) Architecture provides a clean separation of concerns, making the application maintainable, testable, and scalable.

## Key Components

### Model Layer
- **ChessBoard**: Core chess game logic and state
- **ChessGame**: Game metadata and move history
- **ChessPiece**: Individual piece representation
- **Move**: Move representation and validation
- **AppSettings**: Application configuration
- **GameBank**: Game storage and management

### View Layer
- **MainWindow.axaml**: Main application window
- **XAML Templates**: UI component definitions
- **Data Templates**: Data-driven UI elements
- **Styles**: Visual styling and theming

### ViewModel Layer
- **ChessBoardViewModel**: Main application logic
- **SquareViewModel**: Individual square management
- **Property Change Notifications**: Real-time UI updates
- **Command Pattern**: User interaction handling

## Technical Implementation

### ViewModel Base
```csharp
public class ChessBoardViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

### Data Binding
- **Property Binding**: UI elements bound to ViewModel properties
- **Command Binding**: UI actions bound to ViewModel commands
- **Collection Binding**: Lists bound to ObservableCollection
- **Value Converters**: Data transformation for display

### Property Change Notifications
- **INotifyPropertyChanged**: Interface for property change notifications
- **Automatic Updates**: UI updates automatically when properties change
- **Performance Optimization**: Only update changed properties
- **Thread Safety**: Proper thread handling for UI updates

## Architecture Benefits

### Separation of Concerns
- **Model**: Pure business logic and data
- **View**: Pure UI presentation
- **ViewModel**: UI logic and data transformation
- **Clear Boundaries**: Well-defined responsibilities

### Testability
- **Unit Testing**: ViewModels can be tested independently
- **Mocking**: Easy to mock dependencies for testing
- **Isolation**: Business logic separated from UI
- **Automated Testing**: Comprehensive test coverage

### Maintainability
- **Code Organization**: Clear structure and organization
- **Reusability**: Components can be reused
- **Modularity**: Easy to modify individual components
- **Documentation**: Self-documenting code structure

### Scalability
- **Feature Addition**: Easy to add new features
- **Component Replacement**: Easy to replace components
- **Performance**: Optimized for performance
- **Extensibility**: Easy to extend functionality

## Data Flow

### User Interaction Flow
1. **User Action**: User interacts with UI element
2. **Command Execution**: ViewModel command is executed
3. **Model Update**: Model is updated with new data
4. **Property Change**: ViewModel property changes
5. **UI Update**: UI automatically updates via data binding

### Data Update Flow
1. **Model Change**: Model data changes
2. **ViewModel Update**: ViewModel updates its properties
3. **Property Notification**: PropertyChanged event is raised
4. **UI Binding**: UI elements bound to properties update
5. **Visual Update**: User sees updated information

## ViewModel Implementation

### ChessBoardViewModel
- **Game State Management**: Manages current game state
- **UI State Management**: Manages UI state and interactions
- **Command Handling**: Handles user commands and actions
- **Data Transformation**: Transforms model data for UI display

### SquareViewModel
- **Individual Square State**: Manages single square state
- **Piece Information**: Handles piece data and display
- **Selection State**: Manages selection and highlighting
- **Visual Properties**: Handles visual appearance

### Property Management
- **Backing Fields**: Private fields for property values
- **Property Setters**: Public properties with change notification
- **Validation**: Property value validation
- **Dependency Management**: Property dependency handling

## Command Pattern

### Command Implementation
- **ICommand Interface**: Standard command interface
- **CanExecute**: Determines if command can execute
- **Execute**: Executes the command logic
- **CanExecuteChanged**: Notifies when execution state changes

### Command Usage
- **Button Commands**: UI buttons bound to commands
- **Menu Commands**: Menu items bound to commands
- **Keyboard Commands**: Keyboard shortcuts bound to commands
- **Context Commands**: Context menu items bound to commands

## Data Binding

### Property Binding
- **One-Way Binding**: Model to View data flow
- **Two-Way Binding**: Bidirectional data flow
- **One-Time Binding**: Initial value binding
- **Binding Modes**: Different binding modes for different scenarios

### Value Converters
- **ChessPieceImageConverter**: Converts piece data to images
- **BoardSizeDisplayConverter**: Converts size values to display text
- **SquareColorConverter**: Converts position to square color
- **WindowSizeDisplayConverter**: Converts window size to display text

### Collection Binding
- **ObservableCollection**: Notifies UI of collection changes
- **List Binding**: Binding lists to UI elements
- **Item Templates**: Templates for collection items
- **Selection Binding**: Binding selected items

## Error Handling

### Exception Management
- **ViewModel Exceptions**: Handle ViewModel-level errors
- **Model Exceptions**: Handle Model-level errors
- **UI Exceptions**: Handle UI-level errors
- **Graceful Degradation**: Continue operation despite errors

### User Feedback
- **Error Messages**: Display user-friendly error messages
- **Status Updates**: Provide status information
- **Loading Indicators**: Show progress during operations
- **Validation Feedback**: Provide validation feedback

## Dependencies
- System.ComponentModel for INotifyPropertyChanged
- System.Windows.Input for ICommand
- System.Collections.ObjectModel for ObservableCollection
- Avalonia.Data for data binding
