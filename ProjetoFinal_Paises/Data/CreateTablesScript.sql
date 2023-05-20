create table Country
(
    Name_Common              text,
    Name_Official            text,
    Name_NativeName          TEXT,
    Name_NativeName_official TEXT,
    Name_NativeName_common   TEXT,
    Flags_Png                TEXT,
    Flags_Svg                TEXT,
    Flags_Alt                TEXT,
    Continents               TEXT,
    Region                   TEXT,
    SubRegion                TEXT,
    Capital                  TEXT,
    Lat                      double,
    Lng                      double,
    Population               int,
    languages                TEXT,
    languages_full           TEXT,
    currencies               TEXT,
    currencies_name          TEXT,
    currencies_symbol        TEXT,
    UnMember                 boolean,
    Gini_year                int,
    Gini_value               double,
    Cca3                     varchar(3) not null primary key,
    maps_googleMaps          TEXT,
    maps_openStreetMaps      TEXT
);

create table Country_Borders
(
    cca3        varchar(3) not null references Country,
    border_cca3 varchar(3) not null unique,
    primary key (cca3, border_cca3)
);

create table Country_Currencies
(
    cca3            varchar(3) not null references Country,
    Currency        varchar(3) not null unique,
    Currency_Name   text,
    Currency_Symbol text,
    primary key (cca3, Currency)
);


create table Country_Languages
(
    cca3           varchar(3) not null references Country,
    Languages      varchar(3) not null unique,
    Languages_Name text,
    primary key (cca3, Languages)
);

create table Country_Name_NativeName
(
    cca3                varchar(3) not null references Country,
    NativeName          varchar(3) not null unique,
    NativeName_official text       not null unique,
    NativeName_common   text       not null unique,

    primary key (cca3, NativeName)
);

create table Country_Timezones
(
    cca3     varchar(3)  not null references Country,
    timezone varchar(10) not null unique,
    primary key (cca3, timezone)
);


create table Country_Json
(
    country_cca3 varchar(3) not null primary key unique,
    json_data    json       not null
);


drop table if exists Country_Json_1;

create table Country_Json_1
(
    country_cca3 varchar(3) not null primary key unique,
    json_data    json       not null
);

insert into Country_Json_1 (country_cca3, json_data)
select country_cca3, json_data
from Country_Json;


