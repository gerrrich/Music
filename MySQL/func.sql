use mydb;

SET GLOBAL log_bin_trust_function_creators = 1;

drop function if exists `Рейтинг песни`;

DELIMITER //
CREATE FUNCTION `Рейтинг песни` (id int)
RETURNS double
BEGIN
DECLARE mini1 double DEFAULT 0;
DECLARE mini2 double DEFAULT 0;
DECLARE mini3 double DEFAULT 0;
DECLARE mini4 double DEFAULT 0;
DECLARE mini5 double DEFAULT 0;

DECLARE maxi1 double DEFAULT 0;
DECLARE maxi2 double DEFAULT 0;
DECLARE maxi3 double DEFAULT 0;
DECLARE maxi4 double DEFAULT 0;
DECLARE maxi5 double DEFAULT 0;

DECLARE s1 double DEFAULT 0;
DECLARE s2 double DEFAULT 0;
DECLARE s3 double DEFAULT 0;
DECLARE s4 double DEFAULT 0;
DECLARE s5 double DEFAULT 0;

select min(`Просмотры на YouTube`) into mini1 from `песня`;
select min(`Количество лайков на YouTube`) into mini2 from `песня`;
select min(`Количество прослушиваний на Spotify`) into mini3 from `песня`;
select min(`Популярность Deezer`) into mini4 from `песня`;
select min(`Популярность Spotify`) into mini5 from `песня`;

select max(`Просмотры на YouTube`) into maxi1 from `песня`;
select max(`Количество лайков на YouTube`) into maxi2 from `песня`;
select max(`Количество прослушиваний на Spotify`) into maxi3 from `песня`;
select max(`Популярность Deezer`) into maxi4 from `песня`;
select max(`Популярность Spotify`) into maxi5 from `песня`;

select `Просмотры на YouTube` into s1 from `песня` where `ID Песни` = id;
select `Количество лайков на YouTube` into s2 from `песня` where `ID Песни` = id;
select `Количество прослушиваний на Spotify` into s3 from `песня` where `ID Песни` = id;
select `Популярность Deezer` into s4 from `песня` where `ID Песни` = id;
select `Популярность Spotify` into s5 from `песня` where `ID Песни` = id;

set s1 = (s1-mini1)/(maxi1-mini1);
set s2 = (s2-mini2)/(maxi2-mini2);
set s3 = (s3-mini3)/(maxi3-mini3);
set s4 = (s4-mini4)/(maxi4-mini4);
set s5 = (s5-mini5)/(maxi5-mini5);

RETURN (s1+s2+s3+s4+s5)/5;
END//
DELIMITER ;

drop function if exists `Место песни`;

DELIMITER //
CREATE FUNCTION `Место песни` (id int)
RETURNS INTEGER
BEGIN
DECLARE s int DEFAULT 0;
select count(`Рейтинг песни`(`ID Песни`)) into s from `песня` where `Рейтинг песни`(`ID Песни`) > `Рейтинг песни`(id);
#update `песня` set `Место в топе песен` = s+1 where `ID Песни` = id;
RETURN s+1;
END//
DELIMITER ;

drop procedure if exists `Пересчёт топа`;

DELIMITER //
create procedure `Пересчёт топа`()
update `песня` set `Место в топе песен` = `Место песни`(`ID Песни`) where `ID Песни` > 0//
DELIMITER ;

drop trigger if exists `Валидность песни`;

DELIMITER //
create trigger `Валидность песни` before insert 
on `песня` for each row
begin
set new.`Год выхода` = if(new.`Год выхода` >2020, 2020, new.`Год выхода`);
set new.`Популярность Deezer` = if(new.`Популярность Deezer` >10, 10, new.`Популярность Deezer`);
set new.`Популярность Spotify` = if(new.`Популярность Spotify` >10, 10, new.`Популярность Spotify`);
end//
DELIMITER ;

drop trigger if exists `Валидность студии/лейбла`;

DELIMITER //
create trigger `Валидность студии/лейбла` before insert 
on `студия/лейбл` for each row
begin
set new.`Год основания` = if(new.`Год основания` > 2020, 2020, new.`Год основания`);
set new.`Год основания` = if(new.`Год основания` < 1878, 1878, new.`Год основания`);
end//
DELIMITER ;



