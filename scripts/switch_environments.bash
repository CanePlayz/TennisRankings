# Check if blue is running
BLUE_RUNNING=$(docker-compose -f docker-compose_blue.yaml ps | grep Up)

ACTIVE_COMPOSE=""
INACTIVE_COMPOSE=""

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


# Update nginx configuration based on the environment
if [ "$INACTIVE_COMPOSE" == "docker-compose_green.yaml" ]; then
    sed -i '/# BEGIN DEFAULT UPSTREAM/,/# END DEFAULT UPSTREAM/s|proxy_pass http://blue_upstream;|proxy_pass http://green_upstream;|' /etc/nginx/conf.d/tennisrankings.conf
    sed -i '/# BEGIN DEFAULT UPSTREAM/,/# END DEFAULT UPSTREAM/s|proxy_pass http://green_upstream;|proxy_pass http://blue_upstream;|' /etc/nginx/conf.d/tennisrankings.conf
fi

# Reload Nginx Configuration
nginx -s reload

# Stop the old environment
docker-compose -f $ACTIVE_COMPOSE down

# Notify switch completion
echo "Switched from $ACTIVE_COMPOSE to $INACTIVE_COMPOSE successfully!"
