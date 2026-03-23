@timesheet @regression
Feature: Timesheet Management
  As an employee
  I want to manage my timesheet entries
  So that I can submit and verify my work hours

  Background:
    Given the user is logged in and navigated to My Timesheets

  @addTimesheet @smoke
  Scenario: Add a new timesheet entry and verify total hours
    When the user starts adding a new timesheet entry
    And the user enters the project, activity, and hours for the timesheet entry
    And the user saves the timesheet
    Then the timesheet success message is displayed
    And the timesheet entry appears in the table
    And the total hours should be calculated correctly
