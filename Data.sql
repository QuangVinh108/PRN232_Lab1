USE PRN232_Lab1;
GO

-- ===================================================
-- 1. CHÈN ĐỦ 5 HỌC KỲ (Semesters)
-- ===================================================
INSERT INTO Semester (SemesterName, StartDate, EndDate) VALUES
(N'Spring 2025', '2025-01-01', '2025-04-30'),
(N'Summer 2025', '2025-05-01', '2025-08-31'),
(N'Fall 2025',   '2025-09-01', '2025-12-31'),
(N'Spring 2026', '2026-01-01', '2026-04-30'),
(N'Summer 2026', '2026-05-01', '2026-08-31');

-- ===================================================
-- 2. CHÈN ĐỦ 10 MÔN HỌC (Subjects)
-- ===================================================
INSERT INTO Subject (SubjectCode, SubjectName, Credit) VALUES
('PRN211', N'Basic Cross-Platform Application', 3),
('PRN221', N'Advanced Cross-Platform Application', 3),
('PRN231', N'Web Application Development', 3),
('PRN232', N'C# Application Development', 3),
('SWP391', N'Software Development Project', 3),
('SWE201', N'Introduction to Software Engineering', 3),
('DBI202', N'Introduction to Databases', 3),
('PRF192', N'Programming Fundamentals', 3),
('CEA201', N'Computer Organization and Architecture', 3),
('OSG202', N'Operating Systems', 3);

-- ===================================================
-- 3. CHÈN ĐỦ 20 KHÓA HỌC (Courses)
-- Phân bổ đều vào 5 học kỳ (Mỗi học kỳ có 4 khóa học)
-- ===================================================
DECLARE @CourseCount INT = 1;
DECLARE @TargetSemesterId INT;

WHILE @CourseCount <= 20
BEGIN
    -- Vòng lặp lấy SemesterId chạy từ 1 đến 5
    SET @TargetSemesterId = ((@CourseCount - 1) % 5) + 1;
    
    INSERT INTO Course (CourseName, SemesterId)
    VALUES (N'Course Batch ' + CAST(@CourseCount AS VARCHAR(2)), @TargetSemesterId);

    SET @CourseCount = @CourseCount + 1;
END;

-- ===================================================
-- 4. CHÈN ĐỦ 50 HỌC SINH (Students)
-- ===================================================
DECLARE @StudentCount INT = 1;
WHILE @StudentCount <= 50
BEGIN
    INSERT INTO Student (FullName, Email, DateOfBirth)
    VALUES (
        N'Nguyen Van ' + CAST(@StudentCount AS VARCHAR(3)),
        'student' + CAST(@StudentCount AS VARCHAR(3)) + '@fpt.edu.vn',
        DATEADD(DAY, - (7000 + (@StudentCount * 20)), GETDATE()) -- Tính ngày sinh ngẫu nhiên phù hợp lứa tuổi sinh viên
    );
    SET @StudentCount = @StudentCount + 1;
END;

-- ===================================================
-- 5. CHÈN ĐỦ 500 LƯỢT ĐĂNG KÝ (Enrollments)
-- Phân bổ: Mỗi sinh viên trong số 50 sinh viên sẽ đăng ký 10 khóa học khác nhau
-- (50 học sinh * 10 khóa học = 500 bản ghi)
-- ===================================================
DECLARE @StudId INT = 1;
DECLARE @CrsId INT;

WHILE @StudId <= 50
BEGIN
    DECLARE @LoopCount INT = 1;
    -- Tính toán điểm bắt đầu khóa học khác nhau cho từng sinh viên để dữ liệu đan xen nhau
    DECLARE @StartCourseOffset INT = (@StudId % 10); 

    WHILE @LoopCount <= 10
    BEGIN
        -- Tính toán CourseId chạy vòng quanh từ 1 đến 20
        SET @CrsId = ((@StartCourseOffset + @LoopCount - 1) % 20) + 1;

        INSERT INTO Enrollment (StudentId, CourseId, EnrollDate, Status)
        VALUES (
            @StudId, 
            @CrsId, 
            '2025-01-15', 
            CASE WHEN @CrsId % 3 = 0 THEN 'Passed' ELSE 'Enrolled' END
        );

        SET @LoopCount = @LoopCount + 1;
    END
    
    SET @StudId = @StudId + 1;
END;
GO

SELECT 'Semester (Yêu cầu: 5)' AS [Bảng], COUNT(*) AS [Số lượng thực tế] FROM Semester
UNION ALL
SELECT 'Subject (Yêu cầu: 10)', COUNT(*) FROM Subject
UNION ALL
SELECT 'Course (Yêu cầu: 20)', COUNT(*) FROM Course
UNION ALL
SELECT 'Student (Yêu cầu: 50)', COUNT(*) FROM Student
UNION ALL
SELECT 'Enrollment (Yêu cầu: 500)', COUNT(*) FROM Enrollment;