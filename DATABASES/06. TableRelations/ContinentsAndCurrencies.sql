

SELECT MAX(n.CurrencyUsage)
FROM
	(SELECT c.ContinentCode,
			cr.CurrencyCode,
			COUNT(cr.CurrencyCode) AS CurrencyUsage
	 FROM Continents AS c
		INNER JOIN Countries AS cr
			ON c.ContinentCode = cr.ContinentCode
	GROUP BY c.ContinentCode,cr.CurrencyCode
	HAVING COUNT(cr.CurrencyCode) > 1) AS n


