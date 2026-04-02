-- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'HoneyDoThisDb')
BEGIN
    CREATE DATABASE HoneyDoThisDb;
    PRINT 'Database HoneyDoThisDb created successfully.';
END
ELSE
BEGIN
    PRINT 'Database HoneyDoThisDb already exists.';
END
GO

-- Use the database
USE HoneyDoThisDb;
GO

-- Drop existing tables if they exist (in correct order to avoid FK constraints)
DROP TABLE IF EXISTS Subtasks;
DROP TABLE IF EXISTS Tasks;
GO

-- Create Tasks table with constraints
CREATE TABLE Tasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Text NVARCHAR(500) NOT NULL,
    Completed BIT NOT NULL DEFAULT 0,
    [Order] INT NOT NULL,
    Expanded BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    
    -- Constraints
    CONSTRAINT CK_Tasks_Text_NotEmpty CHECK (LEN(TRIM(Text)) > 0),
    CONSTRAINT CK_Tasks_Order_NonNegative CHECK ([Order] >= 0)
);

-- Create indexes for Tasks table
CREATE INDEX IX_Tasks_Order ON Tasks([Order]);
CREATE INDEX IX_Tasks_Completed ON Tasks(Completed);

PRINT 'Table Tasks created successfully with constraints and indexes.';
GO

-- Create Subtasks table with constraints
CREATE TABLE Subtasks (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    TaskId INT NOT NULL,
    Text NVARCHAR(500) NOT NULL,
    Completed BIT NOT NULL DEFAULT 0,
    [Order] INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME2 NULL,
    
    -- Constraints
    CONSTRAINT FK_Subtasks_Tasks FOREIGN KEY (TaskId) 
        REFERENCES Tasks(Id) ON DELETE CASCADE,
    CONSTRAINT CK_Subtasks_Text_NotEmpty CHECK (LEN(TRIM(Text)) > 0),
    CONSTRAINT CK_Subtasks_Order_NonNegative CHECK ([Order] >= 0)
);

-- Create indexes for Subtasks table
CREATE INDEX IX_Subtasks_TaskId ON Subtasks(TaskId);
CREATE INDEX IX_Subtasks_TaskId_Order ON Subtasks(TaskId, [Order]);
CREATE INDEX IX_Subtasks_Completed ON Subtasks(Completed);

PRINT 'Table Subtasks created successfully with constraints and indexes.';
GO

-- Insert sample data into Tasks
INSERT INTO Tasks (Text, Completed, [Order], Expanded, CreatedAt, UpdatedAt)
VALUES 
    ('Grocery Shopping', 0, 0, 0, GETDATE(), NULL),
    ('Home Maintenance', 0, 1, 0, GETDATE(), NULL),
    ('Car Services', 0, 2, 0, GETDATE(), NULL),
    ('Weekly Cleaning', 1, 3, 0, GETDATE(), GETDATE()),
    ('Yard Work', 0, 4, 1, GETDATE(), NULL),
    ('Pet Care', 0, 5, 0, GETDATE(), NULL),
    ('Meal Prep', 0, 6, 0, GETDATE(), NULL),
    ('Bill Payments', 1, 7, 0, GETDATE(), GETDATE()),
    ('Fitness Goals', 0, 8, 0, GETDATE(), NULL),
    ('Family Time Planning', 0, 9, 0, GETDATE(), NULL);

PRINT 'Sample data inserted into Tasks.';
GO

-- Insert sample data into Subtasks (related to Tasks)
INSERT INTO Subtasks (TaskId, Text, Completed, [Order], CreatedAt, UpdatedAt)
VALUES 
    -- Subtasks for "Grocery Shopping" (TaskId 1)
    (1, 'Make a shopping list', 1, 0, GETDATE(), GETDATE()),
    (1, 'Check pantry inventory', 1, 1, GETDATE(), GETDATE()),
    (1, 'Buy vegetables', 0, 2, GETDATE(), NULL),
    (1, 'Buy fruits', 0, 3, GETDATE(), NULL),
    (1, 'Buy dairy products', 0, 4, GETDATE(), NULL),
    (1, 'Buy meat', 0, 5, GETDATE(), NULL),
    (1, 'Buy household items', 0, 6, GETDATE(), NULL),
    
    -- Subtasks for "Home Maintenance" (TaskId 2)
    (2, 'Check HVAC filter', 1, 0, GETDATE(), GETDATE()),
    (2, 'Replace HVAC filter', 0, 1, GETDATE(), NULL),
    (2, 'Test smoke detectors', 1, 2, GETDATE(), GETDATE()),
    (2, 'Replace smoke detector batteries', 0, 3, GETDATE(), NULL),
    (2, 'Clean gutters', 0, 4, GETDATE(), NULL),
    (2, 'Check for leaks under sinks', 0, 5, GETDATE(), NULL),
    
    -- Subtasks for "Car Services" (TaskId 3)
    (3, 'Schedule oil change', 1, 0, GETDATE(), GETDATE()),
    (3, 'Check tire pressure', 1, 1, GETDATE(), GETDATE()),
    (3, 'Rotate tires', 0, 2, GETDATE(), NULL),
    (3, 'Check brake pads', 0, 3, GETDATE(), NULL),
    (3, 'Wash and wax car', 0, 4, GETDATE(), NULL),
    (3, 'Check fluid levels', 0, 5, GETDATE(), NULL),
    
    -- Subtasks for "Weekly Cleaning" (TaskId 4 - Completed)
    (4, 'Vacuum living room', 1, 0, GETDATE(), GETDATE()),
    (4, 'Clean kitchen counters', 1, 1, GETDATE(), GETDATE()),
    (4, 'Mop floors', 1, 2, GETDATE(), GETDATE()),
    (4, 'Clean bathrooms', 1, 3, GETDATE(), GETDATE()),
    (4, 'Take out trash', 1, 4, GETDATE(), GETDATE()),
    (4, 'Dust furniture', 1, 5, GETDATE(), GETDATE()),
    
    -- Subtasks for "Yard Work" (TaskId 5 - Expanded)
    (5, 'Mow lawn', 0, 0, GETDATE(), NULL),
    (5, 'Trim hedges', 0, 1, GETDATE(), NULL),
    (5, 'Pull weeds', 1, 2, GETDATE(), GETDATE()),
    (5, 'Water plants', 1, 3, GETDATE(), GETDATE()),
    (5, 'Fertilize grass', 0, 4, GETDATE(), NULL),
    (5, 'Rake leaves', 0, 5, GETDATE(), NULL),
    (5, 'Clean patio', 0, 6, GETDATE(), NULL),
    
    -- Subtasks for "Pet Care" (TaskId 6)
    (6, 'Buy dog food', 1, 0, GETDATE(), GETDATE()),
    (6, 'Schedule vet appointment', 1, 1, GETDATE(), GETDATE()),
    (6, 'Grooming appointment', 0, 2, GETDATE(), NULL),
    (6, 'Buy new toys', 0, 3, GETDATE(), NULL),
    (6, 'Update pet records', 0, 4, GETDATE(), NULL),
    
    -- Subtasks for "Meal Prep" (TaskId 7)
    (7, 'Plan weekly meals', 1, 0, GETDATE(), GETDATE()),
    (7, 'Create shopping list', 1, 1, GETDATE(), GETDATE()),
    (7, 'Prep vegetables', 0, 2, GETDATE(), NULL),
    (7, 'Cook grains in bulk', 0, 3, GETDATE(), NULL),
    (7, 'Portion meals', 0, 4, GETDATE(), NULL),
    (7, 'Label and store', 0, 5, GETDATE(), NULL),
    
    -- Subtasks for "Bill Payments" (TaskId 8 - Completed)
    (8, 'Check all due dates', 1, 0, GETDATE(), GETDATE()),
    (8, 'Pay electricity bill', 1, 1, GETDATE(), GETDATE()),
    (8, 'Pay water bill', 1, 2, GETDATE(), GETDATE()),
    (8, 'Pay internet bill', 1, 3, GETDATE(), GETDATE()),
    (8, 'Pay credit card', 1, 4, GETDATE(), GETDATE()),
    (8, 'Update payment spreadsheet', 1, 5, GETDATE(), GETDATE()),
    
    -- Subtasks for "Fitness Goals" (TaskId 9)
    (9, 'Create workout plan', 1, 0, GETDATE(), GETDATE()),
    (9, 'Buy workout clothes', 1, 1, GETDATE(), GETDATE()),
    (9, 'Join gym', 1, 2, GETDATE(), GETDATE()),
    (9, 'Schedule daily workouts', 0, 3, GETDATE(), NULL),
    (9, 'Track progress weekly', 0, 4, GETDATE(), NULL),
    (9, 'Meal prep for fitness', 0, 5, GETDATE(), NULL),
    
    -- Subtasks for "Family Time Planning" (TaskId 10)
    (10, 'Discuss weekend plans', 0, 0, GETDATE(), NULL),
    (10, 'Book movie tickets', 0, 1, GETDATE(), NULL),
    (10, 'Plan family dinner', 1, 2, GETDATE(), GETDATE()),
    (10, 'Schedule game night', 0, 3, GETDATE(), NULL),
    (10, 'Coordinate with relatives', 0, 4, GETDATE(), NULL);

PRINT 'Sample data inserted into Subtasks.';
GO

-- Create views for common queries
PRINT 'Creating views...';
GO

-- View: Tasks with subtask counts
CREATE OR ALTER VIEW vw_TasksWithStats AS
SELECT 
    t.Id,
    t.Text,
    t.Completed,
    t.[Order],
    t.Expanded,
    t.CreatedAt,
    t.UpdatedAt,
    COUNT(s.Id) AS SubtaskCount,
    SUM(CASE WHEN s.Completed = 1 THEN 1 ELSE 0 END) AS CompletedSubtaskCount,
    CASE 
        WHEN COUNT(s.Id) > 0 
        THEN (SUM(CASE WHEN s.Completed = 1 THEN 1 ELSE 0 END) * 100 / COUNT(s.Id))
        ELSE 0 
    END AS CompletionPercentage
FROM Tasks t
LEFT JOIN Subtasks s ON t.Id = s.TaskId
GROUP BY t.Id, t.Text, t.Completed, t.[Order], t.Expanded, t.CreatedAt, t.UpdatedAt;
GO

PRINT 'View vw_TasksWithStats created successfully.';
GO

-- Create stored procedures for common operations
PRINT 'Creating stored procedures...';
GO

-- Procedure: Get task with its subtasks
CREATE OR ALTER PROCEDURE sp_GetTaskWithSubtasks
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT * FROM Tasks WHERE Id = @TaskId;
    SELECT * FROM Subtasks WHERE TaskId = @TaskId ORDER BY [Order];
END;
GO

PRINT 'Stored Procedure sp_GetTaskWithSubtasks created successfully.';
GO

-- Procedure: Reorder tasks
CREATE OR ALTER PROCEDURE sp_ReorderTasks
    @TaskId INT,
    @NewOrder INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    DECLARE @OldOrder INT;
    SELECT @OldOrder = [Order] FROM Tasks WHERE Id = @TaskId;
    
    IF @OldOrder IS NOT NULL AND @OldOrder != @NewOrder
    BEGIN
        -- Shift other tasks
        IF @NewOrder > @OldOrder
        BEGIN
            UPDATE Tasks 
            SET [Order] = [Order] - 1
            WHERE [Order] > @OldOrder AND [Order] <= @NewOrder;
        END
        ELSE
        BEGIN
            UPDATE Tasks 
            SET [Order] = [Order] + 1
            WHERE [Order] >= @NewOrder AND [Order] < @OldOrder;
        END
        
        -- Update the moved task
        UPDATE Tasks 
        SET [Order] = @NewOrder,
            UpdatedAt = GETDATE()
        WHERE Id = @TaskId;
    END
    
    COMMIT TRANSACTION;
END;
GO

PRINT 'Stored Procedure sp_ReorderTasks created successfully.';
GO

-- Procedure: Clear completed tasks and reorder
CREATE OR ALTER PROCEDURE sp_ClearCompletedTasks
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    -- Delete completed tasks (cascades to subtasks)
    DELETE FROM Tasks WHERE Completed = 1;
    
    -- Reorder remaining tasks
    WITH TaskOrder AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY [Order]) - 1 AS NewOrder
        FROM Tasks
    )
    UPDATE t
    SET [Order] = TaskOrder.NewOrder,
        UpdatedAt = GETDATE()
    FROM Tasks t
    INNER JOIN TaskOrder ON t.Id = TaskOrder.Id;
    
    COMMIT TRANSACTION;
END;
GO

-- Procedure: Clear completed subtasks for a task
CREATE OR ALTER PROCEDURE sp_ClearCompletedSubtasks
    @TaskId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    -- Delete completed subtasks
    DELETE FROM Subtasks 
    WHERE TaskId = @TaskId AND Completed = 1;
    
    -- Reorder remaining subtasks
    WITH SubtaskOrder AS (
        SELECT Id, ROW_NUMBER() OVER (ORDER BY [Order]) - 1 AS NewOrder
        FROM Subtasks
        WHERE TaskId = @TaskId
    )
    UPDATE s
    SET [Order] = so.NewOrder,
        UpdatedAt = GETDATE()
    FROM Subtasks s
    INNER JOIN SubtaskOrder so ON s.Id = so.Id
    WHERE s.TaskId = @TaskId;
    
    COMMIT TRANSACTION;
END;
GO

PRINT 'Stored Procedure sp_ClearCompletedSubtasks created successfully.';
GO

-- Create functions for common calculations
PRINT 'Creating functions...';
GO

-- Function: Get task completion percentage
CREATE OR ALTER FUNCTION fn_GetTaskCompletionPercentage
(
    @TaskId INT
)
RETURNS INT
AS
BEGIN
    DECLARE @Percentage INT;
    
    SELECT @Percentage = 
        CASE 
            WHEN COUNT(s.Id) > 0 
            THEN (SUM(CASE WHEN s.Completed = 1 THEN 1 ELSE 0 END) * 100 / COUNT(s.Id))
            ELSE 0 
        END
    FROM Tasks t
    LEFT JOIN Subtasks s ON t.Id = s.TaskId
    WHERE t.Id = @TaskId
    GROUP BY t.Id;
    
    RETURN ISNULL(@Percentage, 0);
END;
GO

PRINT 'Function fn_GetTaskCompletionPercentage created successfully.';
GO

-- Verify table structures
PRINT 'Verifying table structures...';
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('Tasks', 'Subtasks')
ORDER BY TABLE_NAME, ORDINAL_POSITION;
GO

-- Verify constraints
PRINT 'Verifying constraints...';
SELECT 
    OBJECT_NAME(parent_object_id) AS TableName,
    name AS ConstraintName,
    type_desc AS ConstraintType
FROM sys.objects
WHERE type_desc LIKE '%CONSTRAINT'
AND OBJECT_NAME(parent_object_id) IN ('Tasks', 'Subtasks')
ORDER BY TableName, ConstraintType;
GO

-- Verify indexes
PRINT 'Verifying indexes...';
SELECT 
    t.name AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique
FROM sys.indexes i
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('Tasks', 'Subtasks')
AND i.name IS NOT NULL
ORDER BY t.name, i.name;
GO

-- Verify inserted data with counts
PRINT 'Verifying inserted data...';
SELECT 'Tasks' AS TableName, COUNT(*) AS RecordCount FROM Tasks
UNION ALL
SELECT 'Subtasks' AS TableName, COUNT(*) AS RecordCount FROM Subtasks;
GO

-- Show sample data with relationships
PRINT 'Sample data with task-subtask relationships:';
SELECT 
    t.Id AS TaskId,
    t.Text AS TaskText,
    t.Completed AS TaskCompleted,
    COUNT(s.Id) AS SubtaskCount,
    SUM(CASE WHEN s.Completed = 1 THEN 1 ELSE 0 END) AS CompletedSubtasks
FROM Tasks t
LEFT JOIN Subtasks s ON t.Id = s.TaskId
GROUP BY t.Id, t.Text, t.Completed, t.[Order]
ORDER BY t.[Order];
GO

-- Show detailed task with subtasks for first task
PRINT 'Detailed view of first task with its subtasks:';
DECLARE @FirstTaskId INT = (SELECT TOP 1 Id FROM Tasks ORDER BY Id);
EXEC sp_GetTaskWithSubtasks @FirstTaskId;
GO

-- Test the completion percentage function
PRINT 'Testing completion percentage function:';
SELECT 
    Id AS TaskId,
    Text AS TaskText,
    dbo.fn_GetTaskCompletionPercentage(Id) AS CompletionPercentage
FROM Tasks
ORDER BY Id;
GO

PRINT 'Script completed successfully. All tables created, constraints applied, and sample data inserted.';