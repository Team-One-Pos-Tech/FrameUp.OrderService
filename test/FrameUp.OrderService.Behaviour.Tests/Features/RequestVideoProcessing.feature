Feature: Request Video Processing

Scenario: Request a video to be processed
    Given I have a video file
    And Define Parameters
    When I request the video to be processed
    Then the video should start processing