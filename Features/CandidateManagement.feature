@candidate @regression
Feature: Candidate Workflow
  As a recruiter
  I want to manage candidate recruitment workflows
  So that I can create, review, and update candidate records

  Background:
    Given the user is logged in and navigated to Recruitment candidates

  @createCandidate @smoke @shortlistCandidate @scheduleInterview
  Scenario: Add new candidate and continue status updates up to interview scheduled
    When the user starts adding a new candidate
    And the user enters all required candidate details
    And the user saves the candidate
    Then the candidate creation success message is displayed
    When the user updates the candidate status to shortlist
    Then the candidate status should be updated
    When the user updates the candidate status to interview scheduled
    Then the candidate status should be updated
