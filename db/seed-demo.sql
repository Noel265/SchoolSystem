-- Demo seed data for the SchoolSystem PostgreSQL database.
-- Safe to run multiple times: records are matched by email, employee number,
-- class name/year, subject code, admission number, and grade identity fields.

BEGIN;

-- A teacher is required because Grades.TeacherId is NOT NULL.
-- This account is only here to satisfy demo data relationships. Create real
-- teacher logins through the app if you need teacher access.
INSERT INTO "Users" ("FullName", "Email", "PasswordHash", "Role", "CreatedAt", "IsActive")
SELECT
    'Demo Teacher',
    'demo.teacher@school.com',
    'seed-only-not-for-login',
    'Teacher',
    NOW(),
    TRUE
WHERE NOT EXISTS (
    SELECT 1 FROM "Users" WHERE "Email" = 'demo.teacher@school.com'
);

INSERT INTO "Teachers" ("FullName", "EmployeeNumber", "PhoneNumber", "Specialization", "HireDate", "IsActive", "UserId")
SELECT
    'Demo Teacher',
    'TCH-DEMO-001',
    '+265 999 000 001',
    'General Studies',
    DATE '2024-01-15',
    TRUE,
    u."Id"
FROM "Users" u
WHERE u."Email" = 'demo.teacher@school.com'
  AND NOT EXISTS (
      SELECT 1 FROM "Teachers" WHERE "EmployeeNumber" = 'TCH-DEMO-001'
  );

-- Classes
INSERT INTO "Classes" ("Name", "SchoolLevel", "AcademicYear", "ClassTeacherId")
SELECT 'Standard 4A', 'Primary', '2026', t."Id"
FROM "Teachers" t
WHERE t."EmployeeNumber" = 'TCH-DEMO-001'
  AND NOT EXISTS (
      SELECT 1 FROM "Classes" WHERE "Name" = 'Standard 4A' AND "AcademicYear" = '2026'
  );

INSERT INTO "Classes" ("Name", "SchoolLevel", "AcademicYear", "ClassTeacherId")
SELECT 'Standard 5B', 'Primary', '2026', t."Id"
FROM "Teachers" t
WHERE t."EmployeeNumber" = 'TCH-DEMO-001'
  AND NOT EXISTS (
      SELECT 1 FROM "Classes" WHERE "Name" = 'Standard 5B' AND "AcademicYear" = '2026'
  );

INSERT INTO "Classes" ("Name", "SchoolLevel", "AcademicYear", "ClassTeacherId")
SELECT 'Form 1A', 'Secondary', '2026', t."Id"
FROM "Teachers" t
WHERE t."EmployeeNumber" = 'TCH-DEMO-001'
  AND NOT EXISTS (
      SELECT 1 FROM "Classes" WHERE "Name" = 'Form 1A' AND "AcademicYear" = '2026'
  );

INSERT INTO "Classes" ("Name", "SchoolLevel", "AcademicYear", "ClassTeacherId")
SELECT 'Form 2B', 'Secondary', '2026', t."Id"
FROM "Teachers" t
WHERE t."EmployeeNumber" = 'TCH-DEMO-001'
  AND NOT EXISTS (
      SELECT 1 FROM "Classes" WHERE "Name" = 'Form 2B' AND "AcademicYear" = '2026'
  );

-- Subjects
INSERT INTO "Subjects" ("Name", "Code", "SchoolLevel", "IsActive")
SELECT 'English', 'ENG-P', 'Primary', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Subjects" WHERE "Code" = 'ENG-P');

INSERT INTO "Subjects" ("Name", "Code", "SchoolLevel", "IsActive")
SELECT 'Mathematics', 'MATH-P', 'Primary', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Subjects" WHERE "Code" = 'MATH-P');

INSERT INTO "Subjects" ("Name", "Code", "SchoolLevel", "IsActive")
SELECT 'Science', 'SCI-P', 'Primary', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Subjects" WHERE "Code" = 'SCI-P');

INSERT INTO "Subjects" ("Name", "Code", "SchoolLevel", "IsActive")
SELECT 'English', 'ENG-S', 'Secondary', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Subjects" WHERE "Code" = 'ENG-S');

INSERT INTO "Subjects" ("Name", "Code", "SchoolLevel", "IsActive")
SELECT 'Mathematics', 'MATH-S', 'Secondary', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Subjects" WHERE "Code" = 'MATH-S');

INSERT INTO "Subjects" ("Name", "Code", "SchoolLevel", "IsActive")
SELECT 'Biology', 'BIO-S', 'Secondary', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Subjects" WHERE "Code" = 'BIO-S');

INSERT INTO "Subjects" ("Name", "Code", "SchoolLevel", "IsActive")
SELECT 'History', 'HIST-S', 'Secondary', TRUE
WHERE NOT EXISTS (SELECT 1 FROM "Subjects" WHERE "Code" = 'HIST-S');

-- Students
INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'Aisha Phiri', 'STU-2026-001', DATE '2015-03-14', 'Female', 'Primary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Standard 4A' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-001');

INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'Blessings Banda', 'STU-2026-002', DATE '2014-11-02', 'Male', 'Primary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Standard 4A' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-002');

INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'Chikondi Mbewe', 'STU-2026-003', DATE '2014-06-23', 'Female', 'Primary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Standard 5B' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-003');

INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'David Mkandawire', 'STU-2026-004', DATE '2013-09-09', 'Male', 'Primary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Standard 5B' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-004');

INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'Esnart Gondwe', 'STU-2026-005', DATE '2011-01-30', 'Female', 'Secondary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Form 1A' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-005');

INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'Francis Zimba', 'STU-2026-006', DATE '2010-12-19', 'Male', 'Secondary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Form 1A' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-006');

INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'Grace Nyirenda', 'STU-2026-007', DATE '2009-04-17', 'Female', 'Secondary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Form 2B' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-007');

INSERT INTO "Students" ("FullName", "AdmissionNumber", "DateOfBirth", "Gender", "SchoolLevel", "EnrollmentDate", "IsActive", "ClassId")
SELECT 'Hope Kamwendo', 'STU-2026-008', DATE '2009-08-28', 'Male', 'Secondary', DATE '2026-01-08', TRUE, c."Id"
FROM "Classes" c
WHERE c."Name" = 'Form 2B' AND c."AcademicYear" = '2026'
  AND NOT EXISTS (SELECT 1 FROM "Students" WHERE "AdmissionNumber" = 'STU-2026-008');

-- Marks / grades. Midterm and final are each out of 50.
WITH grade_rows AS (
    SELECT * FROM (VALUES
        ('STU-2026-001', 'ENG-P', 39.0, 44.0, 'A', 'Excellent reading and writing.'),
        ('STU-2026-001', 'MATH-P', 35.0, 40.0, 'B', 'Good progress in problem solving.'),
        ('STU-2026-001', 'SCI-P', 42.0, 41.0, 'A', 'Strong science curiosity.'),
        ('STU-2026-002', 'ENG-P', 31.0, 34.0, 'C', 'Keep practicing composition.'),
        ('STU-2026-002', 'MATH-P', 37.0, 39.0, 'B', 'Solid number work.'),
        ('STU-2026-002', 'SCI-P', 33.0, 36.0, 'B', 'Participates well in class.'),
        ('STU-2026-003', 'ENG-P', 43.0, 45.0, 'A', 'Excellent oral participation.'),
        ('STU-2026-003', 'MATH-P', 28.0, 32.0, 'C', 'Needs more practice with fractions.'),
        ('STU-2026-003', 'SCI-P', 38.0, 42.0, 'A', 'Very consistent performance.'),
        ('STU-2026-004', 'ENG-P', 29.0, 31.0, 'C', 'Improving steadily.'),
        ('STU-2026-004', 'MATH-P', 41.0, 43.0, 'A', 'Excellent mathematics work.'),
        ('STU-2026-004', 'SCI-P', 30.0, 35.0, 'C', 'Good effort.'),
        ('STU-2026-005', 'ENG-S', 40.0, 42.0, 'A', 'Clear and confident writing.'),
        ('STU-2026-005', 'MATH-S', 34.0, 38.0, 'B', 'Good algebra foundations.'),
        ('STU-2026-005', 'BIO-S', 37.0, 41.0, 'B', 'Strong lab notes.'),
        ('STU-2026-005', 'HIST-S', 35.0, 36.0, 'B', 'Good recall and analysis.'),
        ('STU-2026-006', 'ENG-S', 32.0, 33.0, 'C', 'Needs more essay practice.'),
        ('STU-2026-006', 'MATH-S', 44.0, 46.0, 'A', 'Outstanding mathematics.'),
        ('STU-2026-006', 'BIO-S', 30.0, 31.0, 'C', 'Review terminology.'),
        ('STU-2026-006', 'HIST-S', 39.0, 40.0, 'B', 'Very good class engagement.'),
        ('STU-2026-007', 'ENG-S', 45.0, 47.0, 'A', 'Excellent work overall.'),
        ('STU-2026-007', 'MATH-S', 36.0, 39.0, 'B', 'Consistent improvement.'),
        ('STU-2026-007', 'BIO-S', 41.0, 43.0, 'A', 'Strong understanding.'),
        ('STU-2026-007', 'HIST-S', 42.0, 44.0, 'A', 'Thoughtful answers.'),
        ('STU-2026-008', 'ENG-S', 27.0, 30.0, 'D', 'Needs regular reading practice.'),
        ('STU-2026-008', 'MATH-S', 33.0, 35.0, 'C', 'Making steady progress.'),
        ('STU-2026-008', 'BIO-S', 29.0, 34.0, 'C', 'Good effort in practicals.'),
        ('STU-2026-008', 'HIST-S', 31.0, 32.0, 'C', 'Participates in discussions.')
    ) AS rows(admission_number, subject_code, midterm_score, final_score, letter_grade, comments)
),
resolved AS (
    SELECT
        s."Id" AS student_id,
        sub."Id" AS subject_id,
        t."Id" AS teacher_id,
        gr.midterm_score,
        gr.final_score,
        gr.letter_grade,
        gr.comments
    FROM grade_rows gr
    JOIN "Students" s ON s."AdmissionNumber" = gr.admission_number
    JOIN "Subjects" sub ON sub."Code" = gr.subject_code
    JOIN "Teachers" t ON t."EmployeeNumber" = 'TCH-DEMO-001'
)
INSERT INTO "Grades" (
    "SubjectId",
    "Marks",
    "LetterGrade",
    "MidtermScore",
    "FinalScore",
    "Term",
    "AcademicYear",
    "Comments",
    "StudentId",
    "TeacherId",
    "EnteredDate"
)
SELECT
    r.subject_id,
    r.midterm_score + r.final_score,
    r.letter_grade,
    r.midterm_score,
    r.final_score,
    'Term 1',
    '2026',
    r.comments,
    r.student_id,
    r.teacher_id,
    NOW()
FROM resolved r
WHERE NOT EXISTS (
    SELECT 1
    FROM "Grades" g
    WHERE g."StudentId" = r.student_id
      AND g."SubjectId" = r.subject_id
      AND g."Term" = 'Term 1'
      AND g."AcademicYear" = '2026'
);

COMMIT;
