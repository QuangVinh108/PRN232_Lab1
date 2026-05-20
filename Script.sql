CREATE DATABASE PRN232_Lab1;
GO

USE PRN232_Lab1;
GO

-- 1. Bảng Semester [cite: 6]
CREATE TABLE Semester (
    SemesterId INT PRIMARY KEY IDENTITY(1,1),
    SemesterName NVARCHAR(100) NOT NULL,
    StartDate DATETIME NOT NULL,
    EndDate DATETIME NOT NULL
);

-- 2. Bảng Course [cite: 8]
CREATE TABLE Course (
    CourseId INT PRIMARY KEY IDENTITY(1,1),
    CourseName NVARCHAR(100) NOT NULL,
    SemesterId INT NOT NULL,
    FOREIGN KEY (SemesterId) REFERENCES Semester(SemesterId)
);

-- 3. Bảng Subject [cite: 9]
CREATE TABLE Subject (
    SubjectId INT PRIMARY KEY IDENTITY(1,1),
    SubjectCode VARCHAR(20) NOT NULL,
    SubjectName NVARCHAR(100) NOT NULL,
    Credit INT NOT NULL
);

-- 4. Bảng Student [cite: 10]
CREATE TABLE Student (
    StudentId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    DateOfBirth DATETIME NOT NULL
);

-- 5. Bảng Enrollment [cite: 11]
CREATE TABLE Enrollment (
    EnrollmentId INT PRIMARY KEY IDENTITY(1,1),
    StudentId INT NOT NULL,
    CourseId INT NOT NULL,
    EnrollDate DATETIME NOT NULL,
    Status VARCHAR(20) NOT NULL,
    FOREIGN KEY (StudentId) REFERENCES Student(StudentId),
    FOREIGN KEY (CourseId) REFERENCES Course(CourseId)
);
GO