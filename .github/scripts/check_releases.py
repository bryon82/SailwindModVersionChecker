#!/usr/bin/env python3
"""
Script to check for new releases in GitHub repositories and Thunderstore packages.
"""
import os
import json
import re
import requests
import github
from github import Github

# Configuration files
MOD_LIST_FILE = "ModList.json"
RELEASE_VERSIONS_FILE = "release_versions.json"

# Thunderstore API base URL
THUNDERSTORE_API_URL = "https://thunderstore.io/api/experimental"

def load_mod_list():
    """Load repository list from ModList.json"""
    if os.path.exists(MOD_LIST_FILE):
        with open(MOD_LIST_FILE, 'r') as f:
            return json.load(f)

def extract_repo_info(repo_url):
    """Extract owner and repository name from a GitHub URL"""
    # Handle formats like https://github.com/owner/repo or github.com/owner/repo
    pattern = r"(?:https?://)?(?:www\.)?github\.com[/:]([^/]+)/([^/]+)(?:\.git)?/?$"
    match = re.match(pattern, repo_url)
    if match:
        return f"{match.group(1)}/{match.group(2)}"
    return None

def extract_thunderstore_info(package_url):
    """Extract community, author, and package name from a Thunderstore URL"""
    # or https://thunderstore.io/c/game/p/Author/PackageName/
    pattern = r"(?:https?://)?(?:www\.)?thunderstore\.io/c/([^/]+)/p/([^/]+)/([^/]+)/?.*"

    match = re.match(pattern, package_url)
    if match:
        return {
            "community": match.group(1),
            "author": match.group(2),
            "package": match.group(3)
        }

    return None

def get_latest_github_release(repo_name, g):
    """Get the latest release for a GitHub repository"""
    try:
        repo = g.get_repo(repo_name)
        latest_release = repo.get_latest_release()
        return latest_release.tag_name
    except github.GithubException as e:
        print(f"Error fetching release for GitHub repo {repo_name}: {e}")
        return None

def get_latest_thunderstore_version(package_info):
    """Get the latest version for a Thunderstore package"""
    try:
        community = package_info["community"]
        author = package_info["author"]
        package = package_info["package"]
        url = f"{THUNDERSTORE_API_URL}/package/{author}/{package}/"

        response = requests.get(url)
        if response.status_code == 200:
            package_data = response.json()
            return package_data.get("latest").get("version_number")
        else:
            print(f"Error fetching Thunderstore package {author}/{package}: HTTP {response.status_code}")
            return None
    except Exception as e:
        print(f"Error fetching Thunderstore package {author}/{package}: {e}")
        return None

def generate_versions_json(versions_data):
    """Generate JSON file with guid and version fields for all mods"""
    versions_json = []

    for item in versions_data:
        versions_json.append({
            "guid": item["guid"],
            "version": item["version"]
        })

    with open(RELEASE_VERSIONS_FILE, "w") as f:
        json.dump(versions_json, f, indent=2)

def main():
    github_token = os.environ.get("GITHUB_TOKEN")
    g = None
    if github_token:
        g = Github(github_token)

    mods = load_mod_list()
    all_versions = []

    for mod in mods:
        guid = mod.get("guid")
        repo_url = mod.get("repo")

        if not guid or not repo_url:
            print(f"Skipping invalid mod entry: {mod}")
            continue

        version = "none"

        # Check if this is a GitHub repository
        repo_name = extract_repo_info(repo_url)
        if repo_name:
            if g:
                print(f"Checking GitHub release for {repo_name} (GUID: {guid})")
                latest_version = get_latest_github_release(repo_name, g)
                if latest_version:
                    version = latest_version
                    print(f"Latest GitHub version for {repo_name} (GUID: {guid}): {version}")
            else:
                print("GITHUB_TOKEN not provided, skipping GitHub repository checks")

        # Check if this is a Thunderstore package
        elif "thunderstore.io" in repo_url:
            package_info = extract_thunderstore_info(repo_url)
            if package_info:
                author = package_info["author"]
                package = package_info["package"]
                print(f"Checking Thunderstore package for {author}/{package} (GUID: {guid})")
                latest_version = get_latest_thunderstore_version(package_info)
                if latest_version:
                    version = latest_version
                    print(f"Latest Thunderstore version for {author}/{package} (GUID: {guid}): {version}")
            else:
                print(f"Could not extract Thunderstore package info from URL: {repo_url}")

        else:
            print(f"Unsupported repository URL format: {repo_url}")

        version_entry = {
            "guid": guid,
            "repo": repo_url,
            "version": version
        }
        all_versions.append(version_entry)

    generate_versions_json(all_versions)
    print(f"Generated version information for {len(all_versions)} mods")

if __name__ == "__main__":
    main()
