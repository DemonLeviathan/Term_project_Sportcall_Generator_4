create table Users(
	user_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	username VARCHAR(30) NOT NULL,
	password_hash VARCHAR(300) NOT NULL,
	birthday DATE NOT NULL,
	user_role VARCHAR(15) CHECK (user_role IN ('User', 'Admin'))  DEFAULT 'User' 
);
drop table Users

create table Activities(
	activity_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	activity_name VARCHAR(35) NOT NULL,
	activity_type VARCHAR(35) NOT NULL  
);
drop table Activities

create table User_data( 
	data_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	user_id INT REFERENCES Users(user_id),
	activity_id INT REFERENCES Activities(activity_id),
	date_info DATE DEFAULT CURRENT_DATE,
	weight NUMERIC,
	height numeric	
);
drop table User_data

CREATE TABLE Friendship (
    friend_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user1_id INT REFERENCES Users(user_id),
    user2_id INT REFERENCES Users(user_id),
    friendship_date DATE DEFAULT CURRENT_DATE
);
drop table Friendship


create table Calls (
	call_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
	call_name VARCHAR(30) NOT NULL,
	friend_id INT REFERENCES Friendship(friend_id),
	call_date DATE DEFAULT CURRENT_DATE,
	status VARCHAR(15) CHECK (status IN ('active', 'complited', 'pending')) 
);

drop table Calls;