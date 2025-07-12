import bcrypt

password = "Mugesh1153@"
# Convert to bytes
password_bytes = password.encode('utf-8')

# Generate salt and hash
hashed = bcrypt.hashpw(password_bytes, bcrypt.gensalt())

print("Hashed Password:", hashed.decode())  
