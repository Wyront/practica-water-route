USE waterroute;

DROP PROCEDURE IF EXISTS sp_auth_login;

CREATE PROCEDURE sp_auth_login(
    IN p_email VARCHAR(255),
    IN p_password_plain VARCHAR(255)
)
BEGIN
    DECLARE v_id INT;
    DECLARE v_hash VARCHAR(255);
    DECLARE v_role_id INT;
    DECLARE v_active TINYINT;
    DECLARE v_full_name VARCHAR(255);
    DECLARE v_role_code VARCHAR(50);

    SELECT u.id, u.password_hash, u.role_id, u.active, u.full_name, r.code
    INTO v_id, v_hash, v_role_id, v_active, v_full_name, v_role_code
    FROM users u
    JOIN roles r ON r.id = u.role_id
    WHERE u.email = p_email COLLATE utf8mb4_0900_ai_ci
    LIMIT 1;

    IF v_id IS NULL THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Пользователь не найден';
    END IF;

    IF v_active = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Учётная запись деактивирована';
    END IF;

    SELECT v_id AS user_id, v_full_name AS full_name, v_role_id AS role_id, v_role_code AS role_code, v_hash AS password_hash;
END;