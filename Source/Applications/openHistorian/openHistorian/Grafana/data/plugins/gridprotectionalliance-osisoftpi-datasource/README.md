# PI Web API Datasource for Grafana

This data source provides access to OSIsoft PI and PI-AF data through PI Web API.

![display](https://github.com/GridProtectionAlliance/osisoftpi-grafana/raw/master/docs/img/system_overview.png)

# Usage

## Datasource Configuration

Create a new instance of the data source from the Grafana Data Sources
administration page.

It is recommended to use "proxy" access settings.
You may need to add "Basic" authentication to your PIWebAPI
server configuration and add credentials to the data source settings.

NOTE: If you are using PI-Coresight, it is recommended to create a new
instance of PI Web API for use with this plugin.

See [PI Web API Documentation](https://docs.osisoft.com/bundle/pi-web-api)
for more information on configuring PI Web API.


## Querying via the PI Asset Framework

![elements_and_attributes.png](https://github.com/GridProtectionAlliance/osisoftpi-grafana/raw/master/docs/img/elements_and_attributes.png)

1. Verify that the `PI Point Search` toggle is greyed off
2. In `Element` click `Select AF Database` and choose desired database in list
    * A new ui segment should appear: `Select AF Element`
    * A known bug currently exists where this new ui segment fails. In this case select the `+` in `Attributes` and it will force create the ui segment
3. Click `Select AF Element` and select the desired AF element
4. Repeat step 3 until the desired element is reached
5. Under `Attributes` click the `+` icon to list attributes found in selected element; select attribute from dropdown
    * If list of attributes does not appear begin typing attribute name and attributes should appear
    * This method can also be used to filter through long lists of attributes
6. Repeat step 5 as many times as desired


## Querying via the PI Dataserver (PI Points)

![pi_point_query.png](https://github.com/GridProtectionAlliance/osisoftpi-grafana/raw/master/docs/img/pi_point_query.png)

1. Toggle the `Pi Point Search` on
2. Under `Data Server` click `Select Dataserver` and select desired PI Dataserver
3. Under `PI Points` click the `+` icon to open a text entry field
4. Type the exact name of the desired PI Point; it is NOT case sensitive (`sinusoid` === `SINUSOID` === `sInUsOiD`)
5. Repeat steps 3 - 4 for as many PI Points as desired


# Template Variables

Child elements are the only supported template variables.
Currently, the query interface requires a json query.

An example config is shown below.  
`{"path": "PISERVER\\DatabaseName\\ElementNameWithChildren"}`

![template_setup_1.png](https://github.com/GridProtectionAlliance/osisoftpi-grafana/raw/master/docs/img/template_setup_1.png)


# Event Frames and Annotations

This datasource can use AF Event Frames as annotations.

![event-frame](https://github.com/GridProtectionAlliance/osisoftpi-grafana/raw/master/docs/img/event_frame.png)

Creating an annotation query and use the Event Frame category as the query string.
Color and regex replacement strings for the name are supported.

For example:  
![event-frame-setup-1](https://github.com/GridProtectionAlliance/osisoftpi-grafana/raw/master/docs/img/event_frame_setup_1.png)
![event-frame-setup-2](https://github.com/GridProtectionAlliance/osisoftpi-grafana/raw/master/docs/img/event_frame_setup_2.png)  


# Installation

Install using the grafana-cli or clone the repository directly
into your Grafana plugin directory.

```
grafana-cli plugins install gridprotectionalliance-osisoftpi-datasource
```


# Trademarks

All product names, logos, and brands are property of their respective owners.
All company, product and service names used in this website are for identification purposes only.
Use of these names, logos, and brands does not imply endorsement.

OSIsoft, the OSIsoft logo and logotype, and PI Web API are all trademarks of OSIsoft, LLC.
