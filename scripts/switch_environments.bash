# Check if blue or green environments are running
BLUE_RUNNING=$(docker ps --filter "name=blue-frontend" --filter "status=running" -q)
GREEN_RUNNING=$(docker ps --filter "name=green-frontend" --filter "status=running" -q)

# Determine which environment is active, or none
if [ -n "$BLUE_RUNNING" ]; then
    ACTIVE_COMPOSE="docker-compose_blue.yaml"
    INACTIVE_COMPOSE="docker-compose_green.yaml"
elif [ -n "$GREEN_RUNNING" ]; then
    ACTIVE_COMPOSE="docker-compose_green.yaml"
    INACTIVE_COMPOSE="docker-compose_blue.yaml"
else
    # If neither environment is running, default to starting blue
    echo -e "Neither environment is currently running. Defaulting to start blue.\n"
    ACTIVE_COMPOSE="docker-compose_green.yaml"  # Set green as "active" to be stopped (won't actually stop anything)
    INACTIVE_COMPOSE="docker-compose_blue.yaml"  # Start blue as it's considered "inactive"
fi

# Pull the latest images for the inactive environment
echo -e "Pulling the latest images for the inactive environment...\n"
/usr/local/bin/docker-compose -f $INACTIVE_COMPOSE pull
echo ""

# Start the inactive environment
echo -e "Starting the inactive environment...\n"
/usr/local/bin/docker-compose -f $INACTIVE_COMPOSE up -d
echo ""

# Wait for the helper service to complete its first scraping to avoid delivering old data

# Path to Nginx configuration file
NGINX_CONFIG="/etc/nginx/conf.d/tennisrankings.conf"
TEMP_CONFIG="/etc/nginx/conf.d/tennisrankings_temp.conf"

# Determine the current proxy_pass setting
CURRENT_SETTING=$(grep "proxy_pass" $NGINX_CONFIG | grep -o "http://[a-z_]*")

# Determine the desired setting based on inactive compose
if [ "$INACTIVE_COMPOSE" == "docker-compose_green.yaml" ]; then
    DESIRED_SETTING="http://green_frontend"
elif [ "$INACTIVE_COMPOSE" == "docker-compose_blue.yaml" ]; then
    DESIRED_SETTING="http://blue_frontend"
fi

# Update nginx configuration only if necessary
if [ "$CURRENT_SETTING" != "$DESIRED_SETTING" ]; then
    echo -e "Updating Nginx configuration...\n"
    sudo sed "/# BEGIN DEFAULT UPSTREAM/,/# END DEFAULT UPSTREAM/ { s|proxy_pass $CURRENT_SETTING;|proxy_pass $DESIRED_SETTING;| }" $NGINX_CONFIG > $TEMP_CONFIG
    sudo mv $TEMP_CONFIG $NGINX_CONFIG
    sudo systemctl restart nginx.service
    echo -e "Nginx configuration updated to $DESIRED_SETTING.\n"
else
    echo -e "Nginx configuration already set to $DESIRED_SETTING, no update required.\n"
fi

# If an environment was previously running, stop it
if [ -n "$BLUE_RUNNING" ] || [ -n "$GREEN_RUNNING" ]; then
    echo -e "Stopping the previously active environment...\n"
    /usr/local/bin/docker-compose -f $ACTIVE_COMPOSE down
    echo ""
fi

# Notify switch completion
if [ -n "$BLUE_RUNNING" ] || [ -n "$GREEN_RUNNING" ]; then
    echo "Switched from $ACTIVE_COMPOSE to $INACTIVE_COMPOSE successfully!"
else
    echo "Started $INACTIVE_COMPOSE as no environment was running."
fi
echo ""
