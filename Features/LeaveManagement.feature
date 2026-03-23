@leave @regression
Feature: Leave Management
  As an employee
  I want to apply for leave and track my leave requests
  So that I can verify the request is submitted correctly

  Background:
    Given the user is logged in

  @applyLeave @smoke
  Scenario: Apply leave from JSON data and verify it is pending approval
    When the user adds leave entitlement using the logged in employee name
    And the user navigates to Leave Apply
    And the user fills the leave application form with data from JSON
    And the user submits the leave request
    Then the leave success message is displayed
    When the user navigates to My Leave
    Then the applied leave should appear in My Leave
    When the user filters leave by the configured date range
    Then the leave status should be Pending Approval
