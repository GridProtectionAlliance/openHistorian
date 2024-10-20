# Use the latest Mono image as the base
FROM mono:latest

# Set environment variables
ENV OPENHISTORIAN_VERSION=v2.8.421-beta
ENV OPENHISTORIAN_GROUP=openhistorian
ENV OPENHISTORIAN_USER=admin
ENV OPENHISTORIAN_PASS=admin

# Install dependencies and tools
RUN apt-get update && apt-get install -y \
    wget \
    bsdtar \
    sqlite3 \
    build-essential \
    libpam0g-dev \
    openssl \
    passwd

# Create a new user and group
RUN groupadd -r $OPENHISTORIAN_GROUP && \
    sh -c 'useradd -r -g $OPENHISTORIAN_GROUP -d /home/$OPENHISTORIAN_USER -m -s /bin/bash -p $(openssl passwd -1 $OPENHISTORIAN_PASS) $OPENHISTORIAN_USER'

# Set password aging policies
RUN chage -M 99999 $OPENHISTORIAN_USER && \
    chage -m 0 $OPENHISTORIAN_USER && \
    chage -W 7 $OPENHISTORIAN_USER

# Download and unzip openHistorian POSIX release
RUN wget https://github.com/GridProtectionAlliance/openHistorian/releases/download/${OPENHISTORIAN_VERSION}/openHistorian-POSIX.zip

# Extract using bsdtar (handles Windows backslashes in zip file)
RUN bsdtar -xf openHistorian-POSIX.zip -C /opt

# Remove the downloaded zip file
RUN rm openHistorian-POSIX.zip

# Set the working directory
WORKDIR /opt/openHistorian

# Generate a service certificate
RUN mono MonoGenCert.exe openHistorian

# Enable local user auth for openHistorian
RUN echo "Enabling security for openHistorian..." && \
    cd UnixTools/ && \
    gcc -c -Wall -Werror -fpic GSF.POSIX.c && \
    gcc -shared -o GSF.POSIX.so GSF.POSIX.o -lpam -lpam_misc -lcrypt && \
    cp -v GSF.POSIX.so .. && \
    sed -i.backup -e '/name="SecureRemoteInteractions"/ s/value="False"/value="True"/' ../openHistorian.exe.config

# Add user directly to the configuration database
RUN sqlite3 -line /opt/openHistorian/ConfigurationCache/openHistorian.db "INSERT INTO UserAccount(Name, DefaultNodeID) VALUES('${OPENHISTORIAN_USER}', (SELECT ID FROM Node));" && \
    sqlite3 -line /opt/openHistorian/ConfigurationCache/openHistorian.db "INSERT INTO ApplicationRoleUserAccount(ApplicationRoleID, UserAccountID) VALUES((SELECT ID FROM ApplicationRole WHERE Name = 'Administrator'), (SELECT ID FROM UserAccount WHERE Name = '${OPENHISTORIAN_USER}'));"

# Expose necessary ports
EXPOSE 8180 7175 6175

# Start the openHistorian service with the -RunAsConsole parameter
CMD ["mono", "openHistorian.exe", "-RunAsConsole"]
