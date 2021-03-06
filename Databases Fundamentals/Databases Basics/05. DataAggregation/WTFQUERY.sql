USE Gringotts
GO



Select DepositGroup From
(Select e.DepositGroup, AVG(MagicWandSize) as LongestMagicWand
	  From WizzardDeposits AS e
	  Group By DepositGroup) as A1
	  Join 
(Select MIN(LongestMagicWand) as MinLongestMagic from  

(Select e.DepositGroup, AVG(MagicWandSize) as LongestMagicWand
	  From WizzardDeposits AS e
	  Group By DepositGroup) as A2

 ) as MinAndAVG
 On A1.LongestMagicWand=MinAndAVG.MinLongestMagic
