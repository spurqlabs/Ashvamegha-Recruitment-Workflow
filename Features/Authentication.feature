@authentication @smoke
Feature: Authentication
  As an OrangeHRM user
  I want to log in and log out of the application
  So that my session is validated end to end

  Scenario: Successful login to application
    Given the user launches the application
    When the user logs in with valid credentials
    Then the dashboard should be displayed

  Scenario: Logout and validate session end
    Given the user launches the application
    And the user logs in with valid credentials
    Then the dashboard should be displayed
    When the user logs out of the application
    Then the login page should be displayed
