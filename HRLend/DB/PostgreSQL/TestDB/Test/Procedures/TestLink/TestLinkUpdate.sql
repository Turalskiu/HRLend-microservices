CREATE OR REPLACE PROCEDURE test.test_link__update(
		_id integer,
		_status_id integer
	) AS
	$$		
		BEGIN
			UPDATE test.test_link
			SET 
				status_id = _status_id
			WHERE id = _id;		
		END;
	$$
	LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE test.test_link__closed(
		_id integer,
		_status_id integer,
		_date_closed timestamp(6) with time zone
	) AS
	$$		
		BEGIN
			UPDATE test.test_link
			SET 
				status_id = _status_id,
				date_closed = _date_closed
			WHERE id = _id;		
		END;
	$$
	LANGUAGE plpgsql;


CREATE OR REPLACE PROCEDURE test.test_link__candidate_count_increase(
		_id integer
	) AS
	$$		
		BEGIN
			UPDATE test.test_link 
			SET
				candidate_count = (candidate_count + 1)
			WHERE id =_id;
		END;
	$$
	LANGUAGE plpgsql;

