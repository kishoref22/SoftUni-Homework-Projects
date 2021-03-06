SELECT cu.ContinentCode,
	   co.CurrencyCode,
	   cu.CurrencyUsage
FROM
(SELECT c.ContinentCode,
	   MAX(c.CurrencyUsage) AS CurrencyUsage
FROM
(SELECT c.ContinentCode,
	    c.CurrencyCode,
	    COUNT(c.CurrencyCode) AS CurrencyUsage
FROM Countries AS c
GROUP BY c.ContinentCode, c.CurrencyCode
HAVING COUNT(c.CurrencyCode) > 1) AS c
GROUP BY c.ContinentCode) AS cu
	INNER JOIN (SELECT c.ContinentCode,
					   c.CurrencyCode,
					   COUNT(c.CurrencyCode) AS CurrencyUsage
				FROM Countries AS c
				GROUP BY c.ContinentCode, c.CurrencyCode
				HAVING COUNT(c.CurrencyCode) > 1) AS co
		ON cu.ContinentCode = co.ContinentCode
		AND cu.CurrencyUsage = co.CurrencyUsage
ORDER BY cu.ContinentCode
