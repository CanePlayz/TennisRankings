# Check if blue is running
BLUE_RUNNING=$(docker ps --filter "name=blue-frontend" --filter "status=running" -q)

# Determine which environment is active
if [ -n "$BLUE_RUNNING" ]; then
    ACTIVE_COMPOSE="docker-compose_blue.yaml"
    INACTIVE_COMPOSE="docker-compose_green.yaml"
else
    ACTIVE_COMPOSE="docker-compose_green.yaml"
    INACTIVE_COMPOSE="docker-compose_blue.yaml"
fi

# Start the inactive environment
docker-compose -f $INACTIVE_COMPOSE up -d

# Wait for the helper service to complete its first scraping to avoid delivering old data

# Path to Nginx configuration file
NGINX_CONFIG="/etc/nginx/conf.d/tennisrankings.conf"
TEMP_CONFIG="/etc/nginx/conf.d/tennisrankings_temp.conf"

# Update nginx configuration based on the environment
if [ "$INACTIVE_COMPOSE" == "docker-compose_green.yaml" ]; then
    sed '/# BEGIN DEFAULT UPSTREAM/,/# END DEFAULT UPSTREAM/ { s|proxy_pass http://blue_frontend/|proxy_pass http://green_frontend/| }' $NGINX_CONFIG > $TEMP_CONFIG
elif [ "$INACTIVE_COMPOSE" == "docker-compose_blue.yaml" ]; then
    sed '/# BEGIN DEFAULT UPSTREAM/,/# END DEFAULT UPSTREAM/ { s|proxy_pass http://green_frontend/|proxy_pass http://blue_frontend/| }' $NGINX_CONFIG > $TEMP_CONFIG
fi

# Replace old config file with new one
mv $TEMP_CONFIG $NGINX_CONFIG

# Reload nginx Configuration
sudo systemctl restart nginx.service

# Stop the old environment
docker-compose -f $ACTIVE_COMPOSE down

# Notify switch completion
echo "Switched from $ACTIVE_COMPOSE to $INACTIVE_COMPOSE successfully!"
