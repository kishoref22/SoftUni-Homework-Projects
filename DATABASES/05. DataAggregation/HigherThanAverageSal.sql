USE SoftUni
GO


SELECT TOP 10 e.FirstName,
			  e.LastName,
			  e.DepartmentId
FROM Employees AS e
WHERE e.Salary > 
(SELECT AVG(d.Salary)
FROM Employees AS d
WHERE d.DepartmentID = e.DepartmentID) 


SELECT TOP 10 e.FirstName,
			  e.LastName,
			  e.DepartmentId
FROM Employees AS e

--SELECT name, department_id, salary
--FROM employees e
--WHERE salary > (select avg(salary) from employees e2 where e2.department_id = e.department_id);